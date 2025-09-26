# Chip-8 HUD Overview

## Keyboard Mapping
- Keys follow the classic Chip-8 layout: `1234` / `QWER` / `ASDF` / `ZXCV`.
- The HUD keypad panel mirrors this grid and highlights keys as you press them.
- If a required key is missing on your keyboard, a banner explains the situation and suggests remapping.

## Telemetry Panels
- **Status**: Execution state (running/paused/stepping), current opcode, program counter, and index register.
- **Registers**: V0–VF values (hex), with highlights for changes since the prior tick.
- **Timers**: Delay and sound timers (decimal) with countdown indicators.
- **Stack**: Current stack depth for quick debugging.
- **Audio**: Shows mute state, whether the beep is active, and target frequency.

## Controls
- **Pause/Resume**: Toggles execution state while leaving telemetry visible.
- **Step**: Executes a single instruction when paused, updating telemetry once.
- **Mute**: Silences the audio beep while leaving timers intact.

## Debugging Hooks
- A browser console helper `window.chip8DebugGetSnapshot()` returns the most recent HUD snapshot.
- `window.chip8DebugGetKeyState()` returns the current keypad highlight flags.
