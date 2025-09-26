using System;
using System.Linq;
using Chip8.Emulator.Core.Diagnostics;
using Xunit;

namespace Chip8.Emulator.UnitTests.Hud;

public class HudSnapshotTests
{
    [Fact]
    public void Should_format_registers_and_opcode_in_snapshot()
    {
        var publisher = new EmulatorSnapshotPublisher();
        publisher.UpdateRegisters(new byte[]
        {
            0x0, 0x1, 0x2, 0x3,
            0x4, 0x5, 0x6, 0x7,
            0x8, 0x9, 0xA, 0xB,
            0xC, 0xD, 0xE, 0xF
        });
        publisher.UpdateProgramCounter(0x0200);
        publisher.UpdateIndexRegister(0x0FA0);
        publisher.UpdateCurrentOpcode(0x1234);
        publisher.PublishSnapshot();

        var descriptors = publisher.BuildRegisterDescriptors();

        Assert.Equal(16, descriptors.Count);
        Assert.Equal("V0", descriptors[0].Label);
        Assert.Equal("0x00", descriptors[0].Value);
        Assert.Equal("0x0F", descriptors.Last().Value);
        Assert.All(descriptors, d => Assert.False(d.Highlight));

        var snapshot = publisher.CurrentSnapshot;
        Assert.Equal("0x1234", ($"0x{snapshot.CurrentOpcode:X4}"));
        Assert.Equal(0x0200, snapshot.ProgramCounter);
        Assert.Equal(0x0FA0, snapshot.IndexRegister);
    }

    [Fact]
    public void Should_highlight_registers_that_change_between_ticks()
    {
        var publisher = new EmulatorSnapshotPublisher();
        var registers = new byte[16];
        registers[0] = 0xAA;
        publisher.UpdateRegisters(registers);
        publisher.PublishSnapshot();

        registers[0] = 0xBB;
        publisher.UpdateRegisters(registers);
        var descriptors = publisher.BuildRegisterDescriptors();

        Assert.True(descriptors[0].Highlight);
        Assert.Equal("0xBB", descriptors[0].Value);

        // Publishing snapshot should reset highlight on next inspection when value stable
        publisher.PublishSnapshot();
        descriptors = publisher.BuildRegisterDescriptors();
        Assert.False(descriptors[0].Highlight);
    }
}
