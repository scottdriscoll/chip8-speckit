# Changelog

## Unreleased
### Added
- Chip-8 HUD with live telemetry panels, classic keyboard mapping, and WebAudio beep controls.
- WebAssembly runtime host hooks for HUD snapshot debugging and audio interop.
- Failing regression tests for keyboard mapping, timing determinism, audio channel, HUD telemetry, ROM instrumentation, and performance budgets.
- Playwright configuration scaffolding for browser-level telemetry verification.
- Initial .NET solution (`chip8.sln`) with core, web, and test projects targeting .NET 8.
- Blazor WebAssembly front end with keyboard capture, ROM loader stub, live HUD rendering, and canvas drawing via JS interop.

### Notes
- `dotnet test` currently requires a solution file; add one before running the full test matrix.
- Playwright specs are scaffolded in TypeScript and need a Node workspace before execution.
