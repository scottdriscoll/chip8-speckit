# Data Model – Chip-8 Browser Emulator HUD & Controls

## EmulatorStateSnapshot
- `Registers`: array of 16 bytes representing V0–VF.
- `IndexRegister`: ushort capturing I register.
- `ProgramCounter`: ushort pointing to current opcode address.
- `StackDepth`: byte count of active call frames (max 16).
- `DelayTimer`: byte representing remaining delay timer ticks.
- `SoundTimer`: byte representing remaining sound timer ticks.
- `CurrentOpcode`: ushort of the opcode being executed this cycle.
- `CycleNumber`: running counter incremented each emulation step (used for HUD throttling/testing).
- `ExecutionState`: enum (`Running`, `Paused`, `Stepping`) for HUD controls.

## HudPanelDescriptor
- `PanelId`: enum (`Registers`, `Timers`, `Opcode`, `Stack`, `Keypad`).
- `Title`: display label.
- `DataBindings`: collection describing which fields from `EmulatorStateSnapshot` populate the panel.
- `UpdateCadence`: expected refresh rate (fixed at 60 Hz, expressed as tick number).
- `Format`: formatting rules per data point (hex, decimal, binary, highlight thresholds).
- `Layout`: CSS grid position metadata (row, column spans) for responsive HUD placement.

## KeyboardLayoutProfile
- `Mapping`: dictionary from Chip-8 nibble (0x0–0xF) to keyboard key code (`Digit1`, `KeyQ`, etc.).
- `DisplayLabels`: dictionary from nibble to human-readable label shown in HUD keypad.
- `HighlightStates`: transient structure tracking pressed/released states per tick for HUD feedback.
- `FallbackGuidance`: textual guidance shown when a user lacks mapped keys (e.g., laptops without function rows).

## AudioChannelState
- `IsMuted`: bool toggled via HUD control.
- `Active`: bool indicating whether oscillator currently playing.
- `NextStopTick`: optional tick number when the current beep stops (sound timer hits zero).
- `LastTriggerCycle`: cycle number when beep last started (used for debugging drift).

## HudInteractionSnapshot
- `PauseEnabled`: bool describing whether pause button is available (disabled when already paused).
- `StepPending`: bool toggled when user triggers single-step.
- `ErrorMessage`: optional string surfaced in HUD when ROM load or runtime error occurs.
- `Notifications`: queue of transient HUD messages (e.g., "Audio muted").

## Snapshot Flow
1. Emulator core produces `EmulatorStateSnapshot` each tick.
2. Snapshot is transformed into `HudPanelDescriptor` payloads for UI rendering.
3. `KeyboardLayoutProfile` and `AudioChannelState` feed interactive HUD controls.
4. HUD components render based on descriptors; Playwright tests read the same descriptors via exposed debugging endpoint.
