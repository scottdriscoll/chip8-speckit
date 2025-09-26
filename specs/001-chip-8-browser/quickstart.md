# Quickstart – Chip-8 Browser Emulator HUD & Controls

## Prerequisites
- .NET 8 SDK with WebAssembly workload installed (`dotnet workload install wasm-tools`).
- Node.js 18+ for Playwright test runner.
- Playwright browsers installed (`npx playwright install`).
- VS Code with C# Dev Kit (for debugging) and Playwright Test extensions (optional).

## Build & Run
1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Build the WebAssembly project in Debug:
   ```bash
   dotnet build src/Chip8.Emulator.Web/Chip8.Emulator.Web.csproj -c Debug
   ```
3. Launch the development server (Hot reload enabled):
   ```bash
   dotnet run --project src/Chip8.Emulator.Web/Chip8.Emulator.Web.csproj
   ```
4. Open the browser at `http://localhost:5000` (or port printed in console).
5. In VS Code, use the ".NET Launch (browser)" configuration to attach debugger if needed.

## Manual Verification Steps
- Load the `INVADERS` sample ROM; confirm gameplay renders at 60 fps.
- Press `1`, `2`, `3`, `4` to ensure keypad highlights mirror input.
- Toggle mute control; ensure HUD icon updates and beep stops/resumes.
- Pause emulation; verify HUD freezes, then single-step to observe opcode progression.
- Induce ROM load failure by uploading a non-ROM file; confirm error banner displays without clearing telemetry.

## Automated Tests
1. Run unit tests covering keyboard, timers, and HUD transforms:
   ```bash
   dotnet test tests/Unit/Chip8.Emulator.UnitTests.csproj
   ```
2. Execute ROM regression suite (headless WebAssembly):
   ```bash
   dotnet test tests/Rom/Chip8.Emulator.RomTests.csproj
   ```
3. Launch Playwright integration tests for HUD instrumentation:
   ```bash
   npx playwright test --config=tests/Integration/playwright.config.ts
   ```

## Troubleshooting
- **Audio not playing**: Ensure browser tab has focus and autoplay is permitted; click anywhere in the canvas to grant audio context.
- **Key presses not recognized**: Check OS-level keyboard layout; fallback banner will display alternative suggestions.
- **Performance dips**: Use HUD telemetry overlay to inspect frame time; disable debug logging for release builds.
