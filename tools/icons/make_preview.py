import json
from pathlib import Path
from PIL import Image, ImageDraw

repo = Path(__file__).resolve().parents[2]
root = repo / 'assets' / 'icons' / 'skills'
rows = json.loads((repo / 'docs' / 'assets' / 'skill-icons-prompts-used.json').read_text(encoding='utf-8'))
files = [root / row['file'] for row in rows if (root / row['file']).exists()]
canvas = Image.new('RGB', (7 * 180, ((len(files) + 6) // 7) * 202), '#12101c')
draw = ImageDraw.Draw(canvas)
for i, path in enumerate(files):
    im = Image.open(path)
    assert im.size == (512, 512) and im.format == 'PNG', path
    assert im.mode == 'RGB', path
    x, y = (i % 7) * 180, (i // 7) * 202
    canvas.paste(im.resize((168, 168), Image.Resampling.LANCZOS), (x + 6, y))
    draw.text((x + 5, y + 172), path.stem, fill='white')
canvas.save(Path(__file__).parent / 'preview.jpg', quality=92)  # not committed
print(f'Validated {len(files)} PNG files at 512 x 512, RGB.')
