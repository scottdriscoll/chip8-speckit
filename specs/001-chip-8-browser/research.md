# Phase 0 Research – Chip-8 Browser Emulator HUD & Controls

## Keyboard Layout & HUD Feedback
- **Decision**: Adopt the classic Chip-8 hex keypad arrangement mapped to `1234`, `QWER`, `ASDF`, `ZXCV`, with HUD keypad overlay mirroring this layout and highlighting active keys per emulation tick.
- **Rationale**: The layout is widely recognized by Chip-8 users, works on standard keyboards, and keeps finger travel compact for gameplay. Highlighting based on the same mapping keeps the HUD intuitive and debuggable.
- **Alternatives Considered**:
  - *Direct hexadecimal mapping (`1-0`, `A-F`)*: Rejected because the staggered layout is awkward for fast play and increases mis-press risk.
  - *Configurable bindings persisted to storage*: Deferred per scope; no persistence is required in Phase 1, so simple runtime mapping suffices.

## WebAudio Beep Implementation
- **Decision**: Trigger a short square-wave (Chip-8 style) beep using WebAudio’s `OscillatorNode` when the sound timer decrements from >0 to >0, stopping when the timer reaches zero; expose a mute toggle that short-circuits the oscillator without altering timer logic.
- **Rationale**: Square waves closely resemble original hardware tone, WebAudio provides deterministic playback when scheduled per animation frame, and the approach keeps logic inside existing timer loop.
- **Alternatives Considered**:
  - *HTML5 `<audio>` element playback*: Rejected due to higher latency and limited loop control.
  - *Custom audio buffer synthesis*: Unnecessary for a single beep tone and adds memory overhead.

## HUD Rendering Strategy
- **Decision**: Render HUD panels as Blazor WebAssembly components bound to emulator state snapshots emitted once per tick, supplemented by lightweight CSS grids for layout.
- **Rationale**: Leveraging Blazor avoids manual canvas drawing, keeps updates declarative, and allows conditional formatting (highlighting changes) with minimal custom JS. Snapshot updates per tick keep state deterministic.
- **Alternatives Considered**:
  - *Canvas overlay drawing*: Higher implementation complexity for text and layout, harder to test.
  - *DOM mutations via JS interop*: Increases coupling and risks drifting from single C# core mandate.

## HUD & Input Test Automation
- **Decision**: Use Playwright with the .NET bindings to drive the WebAssembly build in headless Chromium, asserting HUD panel text, keypad highlight states, and audio mute toggle behavior.
- **Rationale**: Playwright integrates with .NET 8, supports capturing rendered text and attributes, and can hook into console logs for emulator telemetry, keeping tests repeatable in CI.
- **Alternatives Considered**:
  - *Manual QA only*: Violates Principle III (test-first coverage).
  - *Selenium/WebDriver*: Heavier setup and slower on headless WebAssembly builds.

## Performance Notes
- The HUD components subscribe to a memoized snapshot to prevent per-field re-render churn.
- Instrumentation updates run on the same 60 Hz timer loop; no additional async timers are introduced, keeping CPU usage predictable.
- Logging for telemetry remains optional in release builds to avoid excess string allocations.

All critical uncertainties for this phase are resolved; proceed to Phase 1 design artifacts.
