# Manual Testing Checklist – Chip-8 HUD & Controls

1. **Launch** the emulator via `dotnet run --project src/Chip8.Emulator.Web/Chip8.Emulator.Web.csproj` and open the browser at the reported localhost port.
2. **Load ROM**: Upload the `INVADERS` sample ROM and confirm gameplay renders at 60 fps.
3. **Keyboard Mapping**: Press `1`, `2`, `3`, `4`, `Q`, `W`, `E`, `R`, `A`, `S`, `D`, `F`, `Z`, `X`, `C`, `V`; verify HUD keypad highlights mirror the layout.
4. **Telemetry Panels**: Observe that registers, timers, opcode, and stack depth update each cycle; confirm changed registers flash.
5. **Pause & Step**: Pause execution, ensure HUD freezes, then single-step and confirm one instruction executes with matching telemetry.
6. **Audio Controls**: Toggle mute; verify beep stops/resumes while the sound timer continues counting down.
7. **ROM Load Failure**: Attempt to load a non-ROM file; confirm error banner displays and prior telemetry remains visible.
8. **Performance**: Watch the HUD frame-time indicator (or browser performance tools) to ensure updates remain responsive.
9. **Debug Hooks**: In browser console, run `chip8DebugGetSnapshot()` and confirm returned JSON includes registers and keypad state.
