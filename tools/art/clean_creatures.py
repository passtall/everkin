"""Make cleaned creature art copies from images/kin (the originals are never changed).

Reads the table in docs/creature-classes.md: for each row it cleans images/kin/<Source> and
writes art/creatures/<Art file> ({unitName}_{attackType}_{professions}.png): 1024 x 1536 RGBA,
transparent background, stray specks removed, the creature scaled to fit the safe area and
standing on the shared ground line. Art files no row names any more are deleted.

Usage: python3 tools/art/clean_creatures.py
"""
import json, sys
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage

ROOT = Path(__file__).resolve().parents[2]
SRC = ROOT / "images" / "kin"
OUT = ROOT / "art" / "creatures"

W, H = 1024, 1536
MARGIN_X = 48          # left and right margin
TOP = 48               # highest point a creature may reach
GROUND = 1456          # shared ground line: the lowest opaque pixel sits here
MIN_ISLAND = 400       # opaque islands smaller than this (in source pixels) are specks

# Leftover background attached to the creature, erased by hand: box in output pixels,
# only near-white pixels inside it are cleared.
WHITE_SPOTS = {"mantis-sickle": (270, 1310, 320, 1375)}


def art_rows():
    """(unit, art file, source file) for each row of the docs/creature-classes.md table."""
    rows = []
    for line in (ROOT / "docs" / "creature-classes.md").read_text(encoding="utf-8").splitlines():
        cells = [c.strip().strip("`") for c in line.strip().strip("|").split("|")]
        if len(cells) == 7 and cells[5].endswith(".png"):
            rows.append((cells[0], cells[5], cells[6]))
    return rows


def clean(src):
    im = Image.open(src).convert("RGBA")
    a = np.array(im)
    alpha = a[:, :, 3]
    labels, n = ndimage.label(alpha > 8)
    sizes = ndimage.sum(np.ones_like(alpha), labels, range(1, n + 1))
    keep = np.isin(labels, [i + 1 for i, s in enumerate(sizes) if s >= MIN_ISLAND])
    a[~keep] = 0
    ys, xs = np.where(a[:, :, 3] > 8)
    box = (xs.min(), ys.min(), xs.max() + 1, ys.max() + 1)
    subject = Image.fromarray(a).crop(box)
    sw, sh = subject.size
    scale = min((W - 2 * MARGIN_X) / sw, (GROUND - TOP) / sh)
    subject = subject.resize((round(sw * scale), round(sh * scale)), Image.LANCZOS)
    canvas = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    x = (W - subject.width) // 2
    canvas.alpha_composite(subject, (x, GROUND - subject.height))
    return canvas, removed_specks(n, sizes), (x, GROUND - subject.height, subject.width, subject.height)


def removed_specks(n, sizes):
    return int(sum(1 for s in sizes if s < MIN_ISLAND))


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    report = {}
    old_report = OUT / "clean_report.json"
    previous = json.loads(old_report.read_text()) if old_report.exists() else {}
    rows = art_rows()
    missing = []
    for unit, art, source in rows:
        src = SRC / source
        if not src.exists():
            print(f"skip {unit}: {src} not found", file=sys.stderr)
            missing.append(unit)
            if unit in previous:
                report[unit] = previous[unit]
            continue
        img, specks, box = clean(src)
        if unit in WHITE_SPOTS:
            a = np.array(img)
            x0, y0, x1, y1 = WHITE_SPOTS[unit]
            region = a[y0:y1, x0:x1]
            region[(region[:, :, :3] > 200).all(axis=2)] = 0
            img = Image.fromarray(a)
        img.save(OUT / art, optimize=True)
        report[unit] = {"file": art, "source": source, "specks_removed": specks, "subject": box}
        print(f"{source} -> {art}  specks removed: {specks}")
    # Prune only files no row names, so a missing source never deletes its existing art.
    for stale in set(p.name for p in OUT.glob("*.png")) - {art for _, art, _ in rows}:
        (OUT / stale).unlink()
        print(f"removed {stale}")
    (OUT / "clean_report.json").write_text(json.dumps(report, indent=1) + "\n")
    if missing:
        sys.exit(f"{len(missing)} source file(s) missing: {', '.join(missing)}")


if __name__ == "__main__":
    main()
