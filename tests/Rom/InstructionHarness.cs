using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Input;
using Chip8.Emulator.Core.Machine;
using Chip8.Emulator.Core.Timing;

namespace Chip8.Emulator.RomTests;

internal sealed class InstructionHarness
{
    private readonly EmulatorSnapshotPublisher _snapshotPublisher = new();
    private readonly TimerService _timerService;
    private readonly Chip8Machine _machine;
    private readonly bool[] _keyStates = new bool[16];

    public InstructionHarness()
    {
        _timerService = new TimerService(_snapshotPublisher);
        _machine = new Chip8Machine(_snapshotPublisher, _timerService);
    }

    public Chip8Machine Machine => _machine;

    public void LoadAndRun(ReadOnlySpan<byte> rom, int cycles)
    {
        _machine.LoadRom(rom);
        RunCycles(cycles);
    }

    public void RunCycles(int cycles)
    {
        for (var i = 0; i < cycles; i++)
        {
            _machine.ExecuteCycles(_keyStates, 1);
            _timerService.ForceTick();
        }
    }

    public void PressKey(byte nibble)
    {
        _keyStates[nibble] = true;
        _machine.SetKeyState(nibble, true);
    }

    public void ReleaseKey(byte nibble)
    {
        _keyStates[nibble] = false;
        _machine.SetKeyState(nibble, false);
    }

    public byte[] SnapshotDisplay()
    {
        var buffer = new byte[Chip8Machine.DisplayWidth * Chip8Machine.DisplayHeight];
        _machine.CopyDisplayTo(buffer);
        return buffer;
    }
}
