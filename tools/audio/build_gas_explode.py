"""Build only the three gas-explosion variants using the existing audio pipeline."""
from pathlib import Path
import json
import build_audio as audio

np = audio.np
ROOT = Path(__file__).resolve().parents[2]

def gas_explode(variant):
    # Independent seeds make each variant reproducible regardless of build order.
    audio.R = np.random.default_rng(13112026 + variant)
    n = round(.6 * audio.SR)
    t = np.arange(n) / audio.SR
    pitch = (1.0, .94, 1.07)[variant - 1]
    # A broad, low filtered-noise pressure puff; no pitched/melodic layer.
    x = .72 * audio.noise(n, 45 * pitch, 420 * pitch) * audio.env(n, .012, 7)
    x += .20 * audio.noise(n, 180, 1500) * audio.env(n, .018, 8)
    # Small, rounded combustion crackles behind the initial whump.
    for at in audio.R.uniform(.055, .28, 8):
        duration = audio.R.uniform(.025, .065)
        count = round(duration * audio.SR)
        crackle = audio.noise(count, 650, 3400) * audio.env(count, .003, 7)
        audio.put(x, crackle, float(at), audio.R.uniform(.035, .075))
    # Explicitly taper the whole tail; no added reverberation.
    x *= np.clip((.56 - t) / .16, 0, 1)
    x[:480] *= np.linspace(0, 1, 480)
    return x



def main():
    audio.records.clear()
    for variant in range(1, 4):
        audio.save(f"audio/battle/gas_explode_{variant}.ogg", gas_explode(variant), -16)
        row = audio.records[-1]
        assert row["duration_s"] == .6
        assert row["sample_rate"] == 48000 and row["channels"] == 1
        assert abs(row["lufs"] + 16) <= .5
        assert row["true_peak_dbtp"] <= -1
    assert len({r["sha256"] for r in audio.records}) == 3
    audio.REPORT.mkdir(parents=True, exist_ok=True)
    (audio.REPORT / "gas_explode-manifest.json").write_text(
        json.dumps(audio.records, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
