# Everkin procedural sound pack

106 original, synthesized game-audio assets, built from the supplied sound list.

- `audio/ui/`: 11 mono interface effects.
- `audio/battle/`: 25 mono battle effects, each with `_1`, `_2`, `_3` variants.
- `audio/result/`: 2 mono, 2-second result stings.
- `audio/ambient/`: 18 stereo, 60-second circular ambient loops.
- All game assets: OGG Vorbis, 48,000 Hz, quality 5.
- `preview_effects.wav`: selected effects, separated by short silences; order in `preview_cues.json`.
- `preview_ambient.wav`: 8 seconds of pine forest, silence, 8 seconds of crystal cavern.
- `manifest.csv` / `manifest.json`: measurements of the actual decoded OGG files, and file hashes.

## Sound direction and limitations

Soft, non-melodic fantasy textures, cushioned impacts, no speech, no sampled recordings, no imported music. Animal calls, water, wind, and material textures are stylized procedural approximations. This is an initial sound-design pass for audition and integration; subjective suitability and mix balance still need listening in the game.

The supplied brief requested AI-generated audio with tool licensing checked. These assets were instead synthesized using AI-authored signal-processing code; no text-to-audio model or hosted audio generation service was used. Natural ambience is deliberately synthetic rather than a field recording.

## Loudness and playback

Effects and result stings target approximately -16 LUFS; ambience targets -23 LUFS. For clips shorter than 3 seconds, loudness is measured on a repeated copy to accommodate the EBU R128 measurement window. This method is documented because standard integrated measurements on clips shorter than 400 ms are not directly defined. Peak protection may make a few transients slightly quieter; exact results are recorded per file. True peaks are estimated with 4x oversampling after Vorbis decoding.

The brief asks for a very faint hover while normalizing all effects equally. The delivered hover shares the effect loudness target; reduce its playback volume in the game to make it faint relative to confirm/cancel.

Loop the entire ambient file from sample 0 to sample 2,879,999 (60 seconds at 48 kHz). Ambient beds are constructed with circular filtering and events wrap across the boundary. The manifest reports the decoded join step relative to normal sample-to-sample changes. Use sample-accurate, gapless audio playback; restarting a new player after each file ends may introduce a gap. No fade-to-silence is baked into the loops.

Battle variants change timbre, event placement, and randomized texture, depending on the effect. The game can apply its small additional random pitch variation.

## Provenance and tool licenses

See `PROVENANCE.md` and `tool_license_metadata.json`. No third-party samples or generation-service assets are included. The build source and pinned package versions are supplied in `tools/`.
