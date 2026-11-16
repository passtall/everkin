#!/usr/bin/env python3
"""Generate content/*.json from the design documents (production.md 2.1, 2.2).

Reads docs/creature-classes.md, docs/class_system.md, docs/skills.md and the named statuses table of
docs/game_system.md section 10.5, plus the
biome list from assets/backgrounds/battle/. Writes content/classes.json,
content/creatures.json, content/skills.json and content/statuses.json.

  python tools/content/build_content.py           write the files
  python tools/content/build_content.py --check   fail if the files differ from the docs

Every error names the file and the ID. The script never guesses: a row that does not
parse, a missing reference or a duplicate ID stops the run.
"""
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "docs"
OUT = ROOT / "content"


class ContentError(Exception):
    pass


def fail(file, ident, message):
    raise ContentError(f"{file}: {ident}: {message}")


def read_tables(path):
    """Yield (heading, preceding_lines, header, rows) for every markdown table of a file."""
    lines = path.read_text(encoding="utf-8").splitlines()
    heading = ""
    before = []
    i = 0
    while i < len(lines):
        line = lines[i]
        if line.startswith("#"):
            heading = line.lstrip("#").strip()
            before = []
        if line.startswith("|") and i + 1 < len(lines) and re.match(r"^\|[\s:|-]+\|$", lines[i + 1]):
            header = split_row(line)
            rows = []
            i += 2
            while i < len(lines) and lines[i].startswith("|"):
                cells = split_row(lines[i])
                if len(cells) != len(header):
                    fail(path.name, f"line {i + 1}", f"{len(cells)} cells, header has {len(header)}")
                rows.append(dict(zip(header, cells)))
                i += 1
            yield heading, before, header, rows
            before = []
            continue
        before.append(line)
        i += 1


def split_row(line):
    return [c.strip() for c in line.strip().strip("|").split("|")]


def unquote(text):
    return text.strip().strip("`").strip()


def slug(text):
    """Snake case ID: Heavy Blow -> heavy_blow (production 2.2)."""
    s = text.lower().replace("'", "").replace("’", "")
    s = re.sub(r"[^a-z0-9]+", "_", s).strip("_")
    return s


def to_int(file, ident, field, text):
    if not re.fullmatch(r"-?\d+", text):
        fail(file, ident, f"{field} is not an integer: {text!r}")
    return int(text)


# ---------------------------------------------------------------- classes

def parse_classes():
    """The 28 classes of class_system.md section 2: role, three actives, three keystones, magical flag."""
    f = "class_system.md"
    text = (DOCS / f).read_text(encoding="utf-8")
    classes = []
    for heading, _, header, rows in read_tables(DOCS / f):
        if header[:2] != ["Class", "Role"]:
            continue
        for r in rows:
            name = r["Class"].strip("*")
            actives = [a.strip() for a in r["Actives"].split(",")]
            keystones = [k.strip() for k in r["Keystones"].split(",")]
            if len(actives) != 3 or len(keystones) != 3:
                fail(f, name, "a class has three actives and three keystones")
            classes.append({"id": slug(name), "name": name, "role": r["Role"], "magical": False,
                            "actives": [slug(a) for a in actives], "keystones": [slug(k) for k in keystones]})
    if len(classes) != 28:
        fail(f, "classes", f"expected 28, found {len(classes)}")
    m = re.search(r"\*\*Magical classes\.\*\*(.+)", text)
    if not m:
        fail(f, "magical classes", "paragraph not found")
    sentence = m.group(1).split(" is magical")[0]
    magical = {n.strip() for n in re.split(r",| and ", sentence.replace("Every active of ", "")) if n.strip()}
    names = {c["name"]: c for c in classes}
    for n in magical:
        if n not in names:
            fail(f, n, "magical class is not a class")
        names[n]["magical"] = True
    if len(magical) != 14:
        fail(f, "magical classes", f"expected 14, found {len(magical)}")
    return classes


# ----------------------------------------------------------------- skills

DM = re.compile(r"^(\d+)\s*/\s*(\d+)$")


def parse_skills(classes):
    """Every class active and keystone of skills.md (84 + 84)."""
    f = "skills.md"
    class_by_name = {c["name"]: c for c in classes}
    skills = []
    for heading, _, header, rows in read_tables(DOCS / f):
        if header[:2] != ["Skill", "Kind"]:
            continue
        if heading not in class_by_name:
            fail(f, heading, "unknown class")
        cls = class_by_name[heading]
        for r in rows:
            name = r["Skill"]
            ident = f"{heading}/{name}"
            kind = r["Kind"]
            if kind not in ("A", "K"):
                fail(f, ident, f"unknown Kind {kind!r}")
            delay = momentum = None
            delivery = None
            if kind == "A":
                m = DM.match(r["D / M"])
                if not m:
                    fail(f, ident, f"cannot read D / M: {r['D / M']!r}")
                delay, momentum = int(m.group(1)), int(m.group(2))
                d = r["Delivery"]
                if d == "—":
                    delivery = "none"
                elif d in ("Melee", "Projectile", "Direct"):
                    delivery = d.lower()
                else:
                    fail(f, ident, f"unknown Delivery {d!r}")
            skills.append({"id": slug(name), "name": name, "class": cls["id"], "kind": kind,
                           "delay": delay, "momentum": momentum, "delivery": delivery,
                           "effect": re.sub(r"\{([^{}]*)\}", r"\g<1>", r["Effect"]), "template": r["Effect"]})
    seen = {}
    for s in skills:
        if s["id"] in seen:
            fail(f, s["id"], "duplicate skill ID")
        seen[s["id"]] = s
    for c in classes:
        for sid, kind in [(a, "A") for a in c["actives"]] + [(k, "K") for k in c["keystones"]]:
            sk = seen.get(sid)
            if sk is None or sk["class"] != c["id"] or sk["kind"] != kind:
                fail(f, f"{c['name']}/{sid}", "listed in class_system.md but missing or different in skills.md")
    if len(skills) != 168:
        fail(f, "skills", f"expected 84 actives and 84 keystones, found {len(skills)}")
    return skills


# ---------------------------------------------------------- unique attacks

STATUS_WORDS = {"Stunned": "stunned", "Confused": "confused", "Immobilized": "immobilized", "Blind": "blind",
                "Silenced": "silenced", "Poisoned": "poisoned"}
GROUND_WORDS = {"poison gas": "poison", "burning ground": "burning", "ice": "ice", "water": "water"}


def parse_attack(file, cid, name, delivery_text, dm, style, effect):
    ident = f"{cid}/{name}"
    delivery = {"Melee": "melee", "Projectile": "projectile", "Direct, melee reach": "direct"}.get(delivery_text)
    if delivery is None:
        fail(file, ident, f"unknown delivery {delivery_text!r}")
    m = DM.match(dm)
    if not m:
        fail(file, ident, f"cannot read D / M: {dm!r}")
    if style not in ("lunge", "swing", "shot", "throw", "bolt", "breath", "pulse"):
        fail(file, ident, f"unknown style {style!r}")
    pulse = re.match(r"^Every enemy in melee reach takes (\d+)–(\d+)(.*)$", effect)
    breath = re.match(r"^Hit the target for (\d+)–(\d+) and every unit next to it for (\d+)–(\d+)(.*)$", effect)
    single = re.match(r"^Hit for (\d+)–(\d+)(.*)$", effect)
    splash = (0, 0)
    if pulse:
        shape, lo, hi, rest = "pulse", int(pulse.group(1)), int(pulse.group(2)), pulse.group(3)
    elif breath:
        shape, lo, hi, rest = "splash", int(breath.group(1)), int(breath.group(2)), breath.group(5)
        splash = (int(breath.group(3)), int(breath.group(4)))
    elif single:
        shape, lo, hi, rest = "single", int(single.group(1)), int(single.group(2)), single.group(3)
    else:
        fail(file, ident, f"cannot read the damage part: {effect!r}")
    if delivery == "direct":
        if shape == "single":
            fail(file, ident, "a Direct attack is a breath or a pulse")
    elif shape == "pulse":
        fail(file, ident, "only Direct attacks pulse")
    rest = rest.strip()
    extra = None
    if rest == ", then the attacker gains stealth.":
        extra = {"kind": "stealth", "chance": 100}
    else:
        em = re.match(r"^;\s*(?:(\d+)%:\s*)?(.+?)\.$", rest)
        if not em:
            fail(file, ident, f"cannot read the extra: {rest!r}")
        chance = int(em.group(1)) if em.group(1) else 100
        body = em.group(2)
        mm = re.fullmatch(r"the target is (\w+) for (\d+) time units", body)
        if mm and mm.group(1) in STATUS_WORDS:
            extra = {"kind": "status", "status": STATUS_WORDS[mm.group(1)], "duration": int(mm.group(2))}
        elif body == "the target Bleeds":
            extra = {"kind": "status", "status": "bleeding", "duration": 0}
        elif body == "the target is Marked":
            extra = {"kind": "status", "status": "marked", "duration": 0}
        elif body == "push the target one row back":
            extra = {"kind": "push"}
        elif body == "pull the target one row toward the battle line":
            extra = {"kind": "pull"}
        elif body == "steal one helpful status from the target":
            extra = {"kind": "steal"}
        else:
            mm = re.fullmatch(r"(.+?) on the target's spot", body)
            if mm and mm.group(1) in GROUND_WORDS:
                extra = {"kind": "ground", "ground": GROUND_WORDS[mm.group(1)]}
            else:
                mm = re.fullmatch(r"the target's next turn comes (\d+) time units later", body)
                if mm:
                    extra = {"kind": "delay_turn", "amount": int(mm.group(1))}
        if extra is None:
            fail(file, ident, f"unknown extra: {body!r}")
        extra["chance"] = chance
    return {"id": slug(name), "name": name, "creature": cid, "delivery": delivery, "shape": shape,
            "delay": int(m.group(1)), "momentum": int(m.group(2)), "style": style,
            "magical": style in ("bolt", "pulse"), "min": lo, "max": hi, "splashMin": splash[0], "splashMax": splash[1],
            "extra": extra, "effect": effect}


# -------------------------------------------------------------- creatures

def parse_frame(file, ident, field, text):
    parts = text.split(",")
    if len(parts) != 4:
        fail(file, ident, f"{field} must be x,y,w,h: {text!r}")
    return [to_int(file, ident, field, p.strip()) for p in parts]


def parse_creatures(classes, skills):
    f = "creature-classes.md"
    class_by_name = {c["name"]: c for c in classes}
    keystone_by_name = {s["name"]: s for s in skills if s["kind"] == "K"}
    skill_ids = {s["id"] for s in skills}
    biomes = {p.stem for p in (ROOT / "assets" / "backgrounds" / "battle").glob("*.png")}
    roster, details = None, None
    for heading, _, header, rows in read_tables(DOCS / f):
        if header[0] == "Unit":
            roster = rows
        elif header[0] == "Art file":
            details = rows
    if roster is None or details is None:
        fail(f, "tables", "roster or card details table not found")
    by_art = {}
    for r in details:
        art = unquote(r["Art file"])
        if art in by_art:
            fail(f, art, "duplicate Art file in Card details")
        by_art[art] = r
    creatures, attacks, seen_ids, used, attack_ids = [], [], set(), set(), set()
    used_keystones = set()
    for r in roster:
        cid = unquote(r["Unit"])
        if cid in seen_ids:
            fail(f, cid, "duplicate unit name")
        seen_ids.add(cid)
        if not re.fullmatch(r"[a-z0-9]+(-[a-z0-9]+)*", cid):
            fail(f, cid, "unit name is not lowercase words joined by hyphens")
        art = unquote(r["Art file"])
        d = by_art.get(art)
        if d is None:
            fail(f, cid, f"no Card details row for art file {art}")
        used.add(art)
        if d["Name"] != r["Name"]:
            fail(f, cid, f"name differs between tables: {r['Name']!r} vs {d['Name']!r}")
        cls = [c.strip() for c in r["Classes"].split(",")]
        if len(cls) != 2 or cls[0] == cls[1]:
            fail(f, cid, f"a creature has two different classes: {cls}")
        keys = [k.strip() for k in r["Keystones"].split(",")]
        if len(keys) != 2:
            fail(f, cid, "a creature has two keystones")
        for c, k in zip(cls, keys):
            if c not in class_by_name:
                fail(f, cid, f"unknown class {c!r}")
            ks = keystone_by_name.get(k)
            if ks is None or ks["class"] != class_by_name[c]["id"]:
                fail(f, cid, f"keystone {k!r} is not a keystone of {c}")
            used_keystones.add(ks["id"])
        biome = slug(d["Biome"])
        if biome not in biomes:
            fail(f, cid, f"biome {d['Biome']!r} has no assets/backgrounds/battle/{biome}.png")
        attack = parse_attack(f, cid, r["Attack"], r["Delivery"], r["D / M"], r["Style"], r["Attack effect"])
        if attack["id"] in attack_ids or attack["id"] in skill_ids:
            fail(f, cid, f"attack ID {attack['id']!r} is already used by another skill")
        attack_ids.add(attack["id"])
        attacks.append(attack)
        creatures.append({
            "id": cid, "name": r["Name"], "classes": [class_by_name[c]["id"] for c in cls],
            "keystones": [keystone_by_name[k]["id"] for k in keys], "attack": attack["id"],
            "artFile": art, "biome": biome,
            "hp": to_int(f, cid, "HP", d["HP"]), "speed": to_int(f, cid, "Speed", d["Speed"]),
            "power": to_int(f, cid, "Power", d["Power"]), "cost": to_int(f, cid, "Cost", d["Cost"]),
            "frontFrame": parse_frame(f, cid, "Front frame", d["Front frame"]),
            "rearFrame": parse_frame(f, cid, "Rear frame", d["Rear frame"]),
        })
    for art in by_art:
        if art not in used:
            fail(f, art, "Card details row without a roster row")
    for k in keystone_by_name.values():
        if k["id"] not in used_keystones:
            fail(f, k["id"], "keystone used by no creature (class_system section 1)")
    for c in creatures:
        if not 1 <= c["cost"] <= 10:
            fail(f, c["id"], f"Cost {c['cost']} outside 1-10")
    creatures.sort(key=lambda c: c["id"])
    attacks.sort(key=lambda a: a["creature"])
    return creatures, attacks


# --------------------------------------------------------------- statuses

def parse_statuses():
    """The named statuses table of game_system.md section 10.5 (Status | Effect | Duration)."""
    f = "game_system.md"
    statuses = []
    for heading, _, header, rows in read_tables(DOCS / f):
        if header != ["Status", "Effect", "Duration"]:
            continue
        for r in rows:
            m = re.match(r"^\*\*(.+?)\*\*(?:\s*\(`(\w+)`\))?$", r["Status"])
            if not m:
                fail(f, r["Status"], "cannot read the status name")
            name = m.group(1)
            statuses.append({"id": slug(name), "name": name, "tag": m.group(2),
                             "effect": r["Effect"], "duration": r["Duration"]})
    if not statuses:
        fail(f, "named statuses", "table not found")
    seen = set()
    for st in statuses:
        if st["id"] in seen:
            fail(f, st["id"], "duplicate status ID")
        seen.add(st["id"])
    return statuses


# ------------------------------------------------------------------- campaign

def parse_campaign(creatures):
    """content/campaign.json from docs/campaign.md: chapters, encounters, enemy teams, rewards (milestone M5.3)."""
    file = "campaign.md"
    by_name = {}
    for c in creatures:
        if c["name"] in by_name:
            fail(file, c["name"], "two creatures share this display name")
        by_name[c["name"]] = c
    biome_ids = {c["biome"] for c in creatures}

    def creature(name, ident):
        if name not in by_name:
            fail(file, ident, f"unknown creature '{name}'")
        return by_name[name]

    chapters = []
    for heading, before, header, rows in read_tables(ROOT / "docs" / file):
        if "Encounter" not in header:
            continue
        if heading.startswith("Tutorial"):
            cid, name = "T", heading.split(":", 1)[1].strip()
        elif heading.startswith("Chapter"):
            cid, name = heading.split(":")[0].split()[1], heading.split(":", 1)[1].strip()
        elif heading.startswith("Challenge"):
            cid, name = "C", "Challenge encounters"
        else:
            fail(file, heading, "unknown table")
        encounters = []
        for row in rows:
            eid = row["#"]
            team = []
            segments = row[[h for h in header if h.startswith("Enemy team")][0]].split(" · ")
            if len(segments) != 3:
                fail(file, eid, "an enemy team needs Front · Middle · Rear")
            for row_name, segment in zip(("Front", "Middle", "Rear"), segments):
                if segment.strip() == "—":
                    continue
                for entry in segment.split(", "):
                    boss = entry.startswith("**")
                    nm = entry.replace("**", "").replace(" (boss)", "").strip()
                    c = creature(nm, eid)
                    team.append({"creature": c["id"], "row": row_name, "boss": boss})
            rewards = [creature(n.strip(), eid)["id"] for n in row["Reward"].split(",")]
            for r in rewards:
                if r not in [t["creature"] for t in team]:
                    fail(file, eid, f"reward {r} is not in the enemy team")
            cost = sum(by_name_id(creatures, t["creature"])["cost"] for t in team)
            if cost != int(row["Team Cost"]):
                fail(file, eid, f"team Cost {cost} does not match the table's {row['Team Cost']}")
            biome = slug(row["Biome"])
            if biome not in biome_ids:
                fail(file, eid, f"unknown biome '{row['Biome']}'")
            ai = row["AI"].strip().lower()
            if ai not in ("easy", "normal", "hard"):
                fail(file, eid, f"unknown AI level '{row['AI']}'")
            encounters.append({"id": eid, "name": row["Encounter"], "biome": biome, "ai": ai, "team": team, "cost": cost, "rewards": rewards})
        chapters.append({"id": cid, "name": name, "biomes": [], "encounters": encounters})
    if [c["id"] for c in chapters] != ["T", "1", "2", "3", "4", "C"]:
        fail(file, "chapters", "expected the tutorial, chapters 1 to 4 and the challenge encounters in this order")
    for ch in chapters:
        ch["biomes"] = sorted({e["biome"] for e in ch["encounters"]})
    return {"chapters": chapters, "bossHpFactorPercent": 150, "bossHpStep": 5, "bossPowerBonus": 2}


def by_name_id(creatures, cid):
    for c in creatures:
        if c["id"] == cid:
            return c
    raise KeyError(cid)


# ------------------------------------------------------------------- main

def build():
    classes = parse_classes()
    skills = parse_skills(classes)
    creatures, attacks = parse_creatures(classes, skills)
    return {
        "classes.json": {"classes": classes},
        "skills.json": {"skills": skills, "attacks": attacks},
        "creatures.json": {"creatures": creatures},
        "statuses.json": {"statuses": parse_statuses()},
        "campaign.json": parse_campaign(creatures),
    }


def render(data):
    return json.dumps(data, ensure_ascii=False, indent=2) + "\n"


def main(argv):
    check = "--check" in argv
    try:
        files = build()
    except ContentError as e:
        print(f"CONTENT ERROR: {e}", file=sys.stderr)
        return 1
    bad = []
    for name, data in files.items():
        text = render(data)
        path = OUT / name
        if check:
            current = path.read_text(encoding="utf-8") if path.exists() else None
            if current != text:
                bad.append(name)
        else:
            OUT.mkdir(exist_ok=True)
            path.write_text(text, encoding="utf-8", newline="\n")
            print(f"wrote {path.relative_to(ROOT)}")
    if bad:
        print(f"CONTENT OUT OF DATE: {', '.join(bad)} differ from the docs; run tools/content/build_content.py",
              file=sys.stderr)
        return 1
    if check:
        print("content matches the docs")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
