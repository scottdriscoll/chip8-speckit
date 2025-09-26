using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Input;
using Chip8.Emulator.Core.Machine;
using Chip8.Emulator.Core.Timing;

namespace Chip8.Emulator.UnitTests.Helpers;

internal sealed class Chip8MachineHarness
{
    private readonly EmulatorSnapshotPublisher _snapshotPublisher = new();
    private readonly TimerService _timerService;

    public Chip8MachineHarness()
    {
        _timerService = new TimerService(_snapshotPublisher);
        Machine = new Chip8Machine(_snapshotPublisher, _timerService);
    }

    public Chip8Machine Machine { get; }

    public void LoadProgram(params byte[] program)
    {
        Machine.LoadRom(program);
    }

    public void Run(int cycles)
    {
        var keys = new bool[16];
        for (var i = 0; i < cycles; i++)
        {
            Machine.ExecuteCycles(keys, 1);
            _timerService.ForceTick();
        }
    }

    public TimerService TimerService => _timerService;
}
