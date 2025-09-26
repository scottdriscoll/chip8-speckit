using System.Collections.Generic;
using Chip8.Emulator.Core.Diagnostics;
using Xunit;

namespace Chip8.Emulator.RomTests.Instrumentation;

public class InstrumentedRomTests
{
    [Fact]
    public void Should_update_hud_panels_while_running_timing_rom()
    {
        var publisher = new EmulatorSnapshotPublisher();
        var observedOpcodes = new List<ushort>();
        publisher.SnapshotCreated += (_, snapshot) => observedOpcodes.Add(snapshot.CurrentOpcode);

        var opcodes = new ushort[] { 0x00E0, 0x6001, 0x6102, 0xA2F0, 0x1200 };

        foreach (var opcode in opcodes)
        {
            publisher.UpdateCurrentOpcode(opcode);
            publisher.UpdateProgramCounter((ushort)(0x200 + observedOpcodes.Count * 2));
            publisher.PublishSnapshot();
        }

        Assert.Equal(opcodes.Length, observedOpcodes.Count);
        Assert.Equal(opcodes, observedOpcodes);
        Assert.Equal(0x1200, publisher.CurrentSnapshot.CurrentOpcode);
    }
}
