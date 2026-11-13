# Gas explosion sound (1)

Played when poison gas explodes (game_system §12.2: fire meets gas, or Lightning Bolt or Chain Lightning hits a unit standing in it). Made 2026-11-13; all three variants are in `assets/audio/battle/`. It joins the battle sounds in [sounds.md](sounds.md) and follows their format: OGG Vorbis 48 kHz mono, about −16 LUFS integrated, **3 variants** `gas_explode_1`, `_2`, `_3` in `assets/audio/battle/`. The existing sounds were synthesized by `tools/audio/build_audio.py` (sounds-notes.md); making this one the same way keeps the set consistent.

## Shared style line (paste after each prompt)

```
Style: soft storybook fantasy game sound, gentle and clean, no music, no melody, no voices, no reverb tail longer than the stated length, no clipping. Suitable for ages 7 and up: impacts are cushioned, never gory.
```

## Sound

| File | Length | Prompt |
|---|---|---|
| `gas_explode` | 0.6 s | A soft muffled whump of gas going up, a short crackle. |

Rebuild with `python tools/audio/build_gas_explode.py`, using the existing sound-pack synthesis and encoder. Decoded measurements are in `tools/audio/report/gas_explode-manifest.json`: 0.6 seconds, 48 kHz mono, −16.3 to −16.5 LUFS and true peaks below −1 dBTP. No subjective listening review has been performed.
