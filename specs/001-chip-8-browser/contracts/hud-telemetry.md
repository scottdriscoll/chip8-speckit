# Contract: HUD Telemetry Panels

## Data Points
- Registers panel MUST display V0–VF values in hexadecimal, updating every emulation tick.
- Timers panel MUST show delay and sound timers in decimal, including a visual countdown indicator when non-zero.
- Opcode panel MUST display the current opcode (hex) and mnemonic string for the cycle being executed.
- Stack panel MUST show current stack depth and top-two return addresses (hex) for debugging.
- Status banner MUST reflect execution state (`Running`, `Paused`, `Stepping`) and recent ROM load errors.

## Update Cadence & Synchronization
- HUD panels MUST refresh after each emulation tick, using the same snapshot consumed by rendering the frame.
- No panel may display data older than one tick relative to the frame; regression test will compare cycle numbers.
- When paused, panels MUST freeze on the last snapshot until resume; single-step MUST apply exactly one tick and update panels once.

## Formatting Rules
- Hex values MUST be zero-padded to 2 digits for bytes and 4 digits for addresses/opcodes.
- Highlight registers that changed since the previous tick (CSS class `.hud-delta`).
- Timers reaching zero MUST clear any countdown highlight immediately.

## Error Handling
- On ROM load failure, display error banner detailing the issue while preserving prior telemetry values.
- If the emulator enters an invalid state (e.g., stack overflow), HUD MUST surface a diagnostic message sourced from emulator logs.

## Test Expectations
- Unit tests MUST validate snapshot-to-panel transformation produces expected strings given sample emulator states.
- Integration tests MUST capture HUD text via Playwright and confirm refresh cadence matches the emulation tick counter.
- Performance budget: rendering HUD panels MUST complete within 3ms per tick on reference hardware to maintain 60 fps.
