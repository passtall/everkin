#!/usr/bin/env python3
"""Generate creature art from the briefs in docs/creature-art-prompt-v2.md with the OpenAI image API.

Setup:  pip install openai   and   set OPENAI_API_KEY
Usage:
  python tools/art/generate_creatures.py --list
  python tools/art/generate_creatures.py --dry-run --only moth-cleric-lantern
  python tools/art/generate_creatures.py --only moth-cleric-lantern          # test one first
  python tools/art/generate_creatures.py --all --quality high
  python tools/art/generate_creatures.py --all --no-refs                     # text-only style (Variant B)

Results go to art/creatures-new/ as {unit}_{attack}_{prof1}_{prof2}.png (existing files are skipped
unless --force). The API returns 1024x1536 PNGs with real alpha; run tools/art/clean_creatures.py
afterwards (after adding rows to docs/creature-classes.md).
"""
import argparse
import base64
import hashlib
import os
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
PROMPT_FILE = ROOT / "docs" / "creature-art-prompt-v2.md"
OUT_DIR = ROOT / "art" / "creatures-new"
DEFAULT_MODEL = "gpt-image-2.5-sunburst"  # editing precision; "gpt-image-2.5-flare" is the fast one
SIZE = "1024x1536"


def slug(text):
    return re.sub(r"[^a-z0-9]+", "-", text.strip().lower()).strip("-")


def parse_prompt_file():
    """Return (variant_a, variant_b_intro, briefs). briefs = list of dicts."""
    text = PROMPT_FILE.read_text(encoding="utf-8")
    a_section = text.split("## Style block, Variant A", 1)[1].split("## Style block, Variant B", 1)
    variant_a = re.search(r"```\n(.*?)```", a_section[0], re.S).group(1).strip()
    variant_b_intro = re.search(r"```\n(.*?)```", a_section[1].split("## Design rules", 1)[0], re.S).group(1).strip()
    # Variant B = Variant A with its first paragraph replaced
    variant_b = variant_b_intro + "\n\n" + variant_a.split("\n\n", 1)[1]

    briefs = []
    for line in text.split("## Briefs", 1)[1].splitlines():
        m = re.match(r"\|\s*`([^`]+)`\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*(.+?)\s*\|$", line)
        if not m:
            continue
        unit, attack, profs, brief = m.groups()
        profession_slugs = [slug(p) for p in profs.split(",")]
        briefs.append({
            "unit": unit,
            "attack": slug(attack),
            "professions": [p.strip() for p in profs.split(",")],
            "brief": brief,
            "file": "_".join([unit, slug(attack)] + profession_slugs) + ".png",
        })
    return variant_a, variant_b, briefs


def favorites():
    """The 11 reference images copied into art/creatures-new/ (everything not made by this script)."""
    _, _, briefs = parse_prompt_file()
    generated = {b["file"] for b in briefs}
    return sorted(p for p in OUT_DIR.glob("*.png") if p.name not in generated)


def pick_refs(unit, refs, count):
    """Deterministic, varied choice of reference images for a unit."""
    if count <= 0 or not refs:
        return []
    start = int(hashlib.md5(unit.encode()).hexdigest(), 16) % len(refs)
    return [refs[(start + i * 3) % len(refs)] for i in range(min(count, len(refs)))]


def build_prompt(b, style):
    profs = " and ".join(b["professions"])
    return (
        f"Creature: {b['brief']}\n"
        f"Role: a {b['attack']} fighter with the professions {profs}; the design should read as that role.\n\n"
        f"{style}"
    )


def save_png(path, data_b64):
    path.write_bytes(base64.b64decode(data_b64))


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--list", action="store_true", help="list the briefs and exit")
    ap.add_argument("--only", nargs="+", metavar="UNIT", help="generate only these unit names")
    ap.add_argument("--all", action="store_true", help="generate every brief")
    ap.add_argument("--dry-run", action="store_true", help="print the prompt and references, call nothing")
    ap.add_argument("--no-refs", action="store_true", help="text-only style (Variant B), no reference images")
    ap.add_argument("--refs", type=int, default=3, help="reference images per request (default 3)")
    ap.add_argument("--model", default=DEFAULT_MODEL)
    ap.add_argument("--quality", default="high", help="low, medium, high, xhigh, max or auto (default high)")
    ap.add_argument("--variants", type=int, default=1, help="images per brief; extras get a _v2, _v3 suffix")
    ap.add_argument("--force", action="store_true", help="overwrite existing files")
    args = ap.parse_args()

    variant_a, variant_b, briefs = parse_prompt_file()
    if args.list:
        for b in briefs:
            print(f"{b['unit']:32} {b['attack']:8} {', '.join(b['professions'])}")
        print(f"{len(briefs)} briefs")
        return
    if args.only:
        known = {b["unit"] for b in briefs}
        unknown = [u for u in args.only if u not in known]
        if unknown:
            sys.exit(f"unknown unit(s): {', '.join(unknown)} (see --list)")
        briefs = [b for b in briefs if b["unit"] in args.only]
    elif not args.all:
        sys.exit("choose --only UNIT [UNIT ...] or --all (or --list)")

    use_refs = not args.no_refs
    refs = favorites() if use_refs else []
    if use_refs and not refs:
        sys.exit(f"no reference images found in {OUT_DIR}")
    style = variant_a if use_refs else variant_b

    client = None
    if not args.dry_run:
        if not os.environ.get("OPENAI_API_KEY"):
            sys.exit("set OPENAI_API_KEY first")
        try:
            from openai import OpenAI
        except ImportError:
            sys.exit("pip install openai")
        client = OpenAI()

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    for b in briefs:
        chosen = pick_refs(b["unit"], refs, args.refs)
        prompt = build_prompt(b, style)
        targets = [OUT_DIR / b["file"]] + [
            OUT_DIR / b["file"].replace(".png", f"_v{i}.png") for i in range(2, args.variants + 1)
        ]
        if args.dry_run:
            print(f"=== {b['unit']} -> {b['file']}")
            print("refs:", ", ".join(p.name for p in chosen) or "none")
            print(prompt, "\n")
            continue
        for target in targets:
            if target.exists() and not args.force:
                print(f"skip   {target.name} (exists)")
                continue
            print(f"make   {target.name} ...", flush=True)
            common = dict(model=args.model, prompt=prompt, size=SIZE, quality=args.quality,
                          background="transparent", output_format="png")
            try:
                if chosen:
                    handles = [open(p, "rb") for p in chosen]
                    try:
                        result = client.images.edit(image=handles, **common)
                    finally:
                        for h in handles:
                            h.close()
                else:
                    result = client.images.generate(**common)
                save_png(target, result.data[0].b64_json)
            except Exception as e:  # keep going so one failure does not lose the batch
                print(f"FAILED {target.name}: {e}")


if __name__ == "__main__":
    main()
