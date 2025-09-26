# Contract: Input Keypad Mapping & Feedback

## Purpose
Guarantee reliable keyboard input for all Chip-8 keys while presenting a HUD keypad that mirrors the classic 4×4 hex layout and highlights active keys.

## Mapping Requirements
| Chip-8 Key | Keyboard Key | HUD Label |
|------------|--------------|-----------|
| 0x1        | `Digit1`     | 1         |
| 0x2        | `Digit2`     | 2         |
| 0x3        | `Digit3`     | 3         |
| 0xC        | `Digit4`     | 4         |
| 0x4        | `KeyQ`       | Q         |
| 0x5        | `KeyW`       | W         |
| 0x6        | `KeyE`       | E         |
| 0xD        | `KeyR`       | R         |
| 0x7        | `KeyA`       | A         |
| 0x8        | `KeyS`       | S         |
| 0x9        | `KeyD`       | D         |
| 0xE        | `KeyF`       | F         |
| 0xA        | `KeyZ`       | Z         |
| 0x0        | `KeyX`       | X         |
| 0xB        | `KeyC`       | C         |
| 0xF        | `KeyV`       | V         |

## Behavioral Rules
- Input mapper MUST debounce keydown/keyup events so each emulation tick receives deterministic state.
- HUD keypad MUST highlight active keys within the same tick they are pressed and clear highlight on release.
- When the browser lacks a mapped key (detected via `KeyboardEvent.code` not firing), HUD MUST display fallback guidance banner without crashing.
- Pause/step operations MUST snapshot keypad state as part of emulator pause to keep highlights consistent.

## Test Hooks
- Expose a debug endpoint (e.g., `window.chip8Debug.getKeyState()`) returning the 16-key state for Playwright assertions.
- Unit test fixture MUST verify the mapper sets correct nibble values given simulated keyboard events.
- Integration test MUST simulate key presses for a ROM and assert HUD highlight order matches expected sequence.
