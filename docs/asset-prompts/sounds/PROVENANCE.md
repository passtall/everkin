# Provenance

Created for the user's Everkin project from the supplied sound-effect list. All waveforms are computed from oscillators, filtered pseudorandom noise, envelopes, and synthesized events. No existing game assets, recordings, sample packs, voice models, or music were used.

The procedural source was authored in this chat. No separate stock-audio license, paid model subscription, or sampled-performance permission was involved. This document records provenance rather than making a copyright ownership or exclusivity guarantee.

## Installed tools inspected

License declarations were read from the installed Python distribution metadata and the encoder's own `-L` output; complete collected metadata is included in `tool_license_metadata.json`.

- NumPy 2.5.4: BSD-3-Clause and bundled-component notices.
- SciPy 1.18.1: BSD-style main license and bundled-component notices.
- SoundFile 0.14.0: BSD 3-Clause; used for decoding and preview WAV writing.
- pyloudnorm 0.2.0: MIT; used for loudness measurements.
- imageio-ffmpeg 0.8.1: BSD-2-Clause wrapper.
- Bundled FFmpeg 9.0.2 build: reports GPL version 3 or later; used as a separate encoder process with libvorbis.

Only generated audio, documentation, build source, and license metadata are delivered. None of these tool binaries or library distributions are included. The software-license declarations above describe the tools, and are not a newly imposed license on the generated audio. If redistributing a tool binary later, consult that tool's full distribution notices separately.
