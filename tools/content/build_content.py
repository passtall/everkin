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
    f = "class_system.md"
    attack_types, professions = [], []
    for heading, _, header, rows in read_tables(DOCS / f):
        if header[:2] == ["Attack Type", "Description"]:
            for r in rows:
                name = r["Attack Type"].strip("*")
                attack_types.append({"id": slug(name), "name": name, "description": r["Description"]})
        elif header[0] == "Profession":
            for r in rows:
                name = r["Profession"].strip("*")
                professions.append({"id": slug(name), "name": name, "description": r["Description and included archetypes"]})
    if len(attack_types) != 9:
        fail(f, "attack types", f"expected 9, found {len(attack_types)}")
    if len(professions) != 12:
        fail(f, "professions", f"expected 12, found {len(professions)}")
    return attack_types, professions


# ----------------------------------------------------------------- skills

DM = re.compile(r"^(\d+)\s*/\s*(\d+)$")


def parse_dm(file, ident, text, required):
    if not text:
        if required:
            fail(file, ident, "Delay / Momentum missing")
        return None, None
    m = DM.match(text)
    if not m:
        fail(file, ident, f"cannot read D / M: {text!r}")
    return int(m.group(1)), int(m.group(2))


def parse_delivery(file, ident, text):
    if not text:
        return None, None
    if text == "As parent":
        return "inherit", None
    m = re.match(r"^(Melee|Projectile|Direct|None)(?:\s*·\s*(.+))?$", text)
    if not m:
        fail(file, ident, f"cannot read Delivery · Range: {text!r}")
    return m.group(1).lower(), (m.group(2) or None)


def parse_skills(attack_types, professions):
    f = "skills.md"
    text = (DOCS / f).read_text(encoding="utf-8")
    at_ids = {a["name"]: a["id"] for a in attack_types}
    prof_ids = {p["name"]: p["id"] for p in professions}
    attack_skills, skills, branches = [], [], {}
    # branch lists: the "Branches: ..." line below each profession heading
    current = None
    for line in text.splitlines():
        m = re.match(r"^## (\w+) \((\d+)\)$", line)
        if m:
            current = m.group(1)
            branches[current] = {"count": int(m.group(2)), "names": []}
        m = re.match(r"^Branches: (.+)\.$", line)
        if m and current:
            branches[current]["names"] = [b.strip() for b in m.group(1).split(",")]
    for heading, _, header, rows in read_tables(DOCS / f):
        if header[0] == "Attack Type":
            for r in rows:
                at = r["Attack Type"]
                if at not in at_ids:
                    fail(f, at, "unknown Attack Type")
                name = r["Attack"]
                d, m_ = parse_dm(f, name, r["D / M"], True)
                attack_skills.append({"id": slug(name), "name": name, "attackType": at_ids[at],
                                      "delay": d, "momentum": m_, "effect": r["Effect"]})
        elif header[0] == "Branch":
            prof = heading.split(" (")[0]
            if prof not in prof_ids:
                fail(f, heading, "unknown profession")
            for r in rows:
                name = r["Skill"]
                ident = f"{prof}/{name}"
                if r["Branch"] not in branches[prof]["names"]:
                    fail(f, ident, f"branch {r['Branch']!r} is not in the profession's Branches line")
                kind_text = r["Kind"]
                modifier_of = None
                placement = None
                mm = re.match(r"^Mod of (.+)$", kind_text)
                if mm:
                    kind, modifier_of = "Mod", slug(mm.group(1))
                elif kind_text in ("A", "P", "K"):
                    kind = kind_text
                elif re.fullmatch(r"A \((trap card|construct|summon|totem)\)", kind_text):
                    kind = "A"
                    placement = kind_text[3:-1].replace(" ", "_")
                else:
                    fail(f, ident, f"unknown Kind {kind_text!r}")
                d, m_ = parse_dm(f, ident, r["D / M"], kind == "A")
                delivery, rng = parse_delivery(f, ident, r["Delivery · Range"])
                needs = None if r["Needs"] in ("—", "-", "") else slug(r["Needs"])
                if needs is None and kind not in ("A", "P", "K", "Mod"):
                    fail(f, ident, "starting skill with odd kind")
                skills.append({
                    "id": slug(name), "name": name, "profession": prof_ids[prof],
                    "branch": r["Branch"], "kind": kind,
                    "placement": placement,
                    "modifierOf": modifier_of, "parent": needs,
                    "delay": d, "momentum": m_, "delivery": delivery, "range": rng,
                    "effect": r["Effect"],
                })
    # validation
    by_id = {}
    for s in skills + attack_skills:
        if s["id"] in by_id:
            fail(f, s["id"], "duplicate skill ID")
        by_id[s["id"]] = s
    count = {}
    for s in skills:
        count[s["profession"]] = count.get(s["profession"], 0) + 1
        for field in ("parent", "modifierOf"):
            ref = s[field]
            if ref is None:
                continue
            target = by_id.get(ref)
            if target is None or "profession" not in target or target["profession"] != s["profession"]:
                fail(f, s["id"], f"{field} {ref!r} is not a skill of the same profession")
        if s["modifierOf"] and s["parent"] != s["modifierOf"]:
            fail(f, s["id"], f"modifier of {s['modifierOf']!r} must need it as parent, needs {s['parent']!r}")
    for prof, info in branches.items():
        pid = prof_ids[prof]
        if count.get(pid, 0) != info["count"]:
            fail(f, prof, f"heading says {info['count']} skills, table has {count.get(pid, 0)}")
    # trees: parent chains must end at a starting skill, without cycles, inside one branch
    for s in skills:
        seen, cur = set(), s
        while cur["parent"]:
            if cur["id"] in seen:
                fail(f, s["id"], "parent cycle")
            seen.add(cur["id"])
            parent = by_id[cur["parent"]]
            if parent["branch"] != s["branch"]:
                fail(f, s["id"], f"parent {parent['id']!r} is in another branch")
            cur = parent
    return attack_skills, skills, {p: branches[p]["names"] for p in branches}


# -------------------------------------------------------------- creatures

def parse_frame(file, ident, field, text):
    parts = text.split(",")
    if len(parts) != 4:
        fail(file, ident, f"{field} must be x,y,w,h: {text!r}")
    return [to_int(file, ident, field, p.strip()) for p in parts]


def parse_creatures(attack_types, professions, skills_by_profession):
    f = "creature-classes.md"
    at_ids = {a["name"]: a["id"] for a in attack_types}
    prof_ids = {p["name"]: p["id"] for p in professions}
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
    creatures, seen_ids, used = [], set(), set()
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
        at = r["Attack type"]
        if at not in at_ids:
            fail(f, cid, f"unknown Attack type {at!r}")
        profs = [p.strip() for p in r["Professions"].split(",")]
        if not 1 <= len(profs) <= 2 or len(set(profs)) != len(profs):
            fail(f, cid, f"a creature has one or two different professions: {profs}")
        for p in profs:
            if p not in prof_ids:
                fail(f, cid, f"unknown profession {p!r}")
        biome = slug(d["Biome"])
        if biome not in biomes:
            fail(f, cid, f"biome {d['Biome']!r} has no assets/backgrounds/battle/{biome}.png")
        source = unquote(r["Source"])
        creatures.append({
            "id": cid, "name": r["Name"], "attackType": at_ids[at],
            "professions": [prof_ids[p] for p in profs],
            "artFile": art, "source": None if source in ("—", "-") else source,
            "biome": biome,
            "hp": to_int(f, cid, "HP", d["HP"]), "speed": to_int(f, cid, "Speed", d["Speed"]),
            "power": to_int(f, cid, "Power", d["Power"]), "cost": to_int(f, cid, "Cost", d["Cost"]),
            "frontFrame": parse_frame(f, cid, "Front frame", d["Front frame"]),
            "rearFrame": parse_frame(f, cid, "Rear frame", d["Rear frame"]),
            "status": d["Status"], "notes": d["Notes"],
        })
    for art in by_art:
        if art not in used:
            fail(f, art, "Card details row without a roster row")
    for c in creatures:
        if not 1 <= c["cost"] <= 10:
            fail(f, c["id"], f"Cost {c['cost']} outside 1-10")
        if c["status"] not in ("reviewed", "draft"):
            fail(f, c["id"], f"unknown Status {c['status']!r}")
    creatures.sort(key=lambda c: c["id"])
    return creatures


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


# ------------------------------------------------------------------- main

def build():
    attack_types, professions = parse_classes()
    attack_skills, skills, branches = parse_skills(attack_types, professions)
    prof_branches = {p["name"]: branches[p["name"]] for p in professions}
    for p in professions:
        p["branches"] = prof_branches[p["name"]]
    creatures = parse_creatures(attack_types, professions, None)
    return {
        "classes.json": {"attackTypes": attack_types, "professions": professions},
        "skills.json": {"attackSkills": attack_skills, "professionSkills": skills},
        "creatures.json": {"creatures": creatures},
        "statuses.json": {"statuses": parse_statuses()},
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
