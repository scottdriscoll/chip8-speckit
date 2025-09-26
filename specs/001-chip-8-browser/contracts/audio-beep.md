# Contract: Timer-Driven Audio Beep

## Trigger Conditions
- When `SoundTimer` > 0, the audio channel MUST emit a continuous beep until the timer reaches 0.
- Beep MUST start within the same tick the timer transitions from 0 → positive.
- When the timer reaches 0, audio MUST stop no later than the next animation frame (16.6 ms at 60 Hz).

## Sound Characteristics
- Waveform: Square wave approximating classic Chip-8 tone (~440 Hz).
- Volume: Fixed comfortable level with optional HUD indicator; no volume persistence required.
- Mute toggle: When muted, oscillator MUST NOT start but timers continue decrementing.

## Controls & HUD Integration
- HUD MUST expose a mute/unmute toggle reflecting `IsMuted` state.
- When muted, HUD displays an icon and retains visibility into timer countdown.
- Playwright tests MUST confirm toggle updates both HUD icon and audio channel state.

## Error Handling
- If WebAudio context fails to start (e.g., user gesture required), HUD MUST prompt user to enable audio and keep emulator running silently.
- Audio errors MUST NOT break gameplay; they are logged for diagnostics.

## Test Expectations
- Unit tests MUST verify audio channel logic triggers beep start/stop given synthetic timer transitions.
- Integration tests MUST mock WebAudio to assert oscillator creation/stop occur on schedule.
- Performance budget: audio control updates MUST avoid creating new oscillator nodes per tick; reuse existing node when possible.
