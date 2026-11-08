import json
from pathlib import Path
from PIL import Image, ImageDraw

root = Path(__file__).parent
rows = json.loads((root / 'prompts.json').read_text(encoding='utf-8'))
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
canvas.save(root / 'preview.jpg', quality=92)
print(f'Validated {len(files)} PNG files at 512 x 512, RGB.')
