using Chip8.Emulator.UnitTests.Helpers;
using Chip8.Emulator.Core.Machine;
using Xunit;

namespace Chip8.Emulator.UnitTests.Machine;

public class Chip8MachineTests
{
    [Fact]
    public void ArithmeticAndLogicInstructionsProduceExpectedResults()
    {
        var harness = new Chip8MachineHarness();
        harness.LoadProgram(
            0x60, 0xFF, // V0 = 0xFF
            0x70, 0x02, // V0 += 0x02 → wraps to 0x01
            0x61, 0x0F, // V1 = 0x0F
            0x80, 0x14, // V0 = V0 + V1, VF = carry (0)
            0x62, 0x01, // V2 = 0x01
            0x82, 0x05  // V2 = V2 - V0 → borrow -> VF = 0
        );

        harness.Run(6);
        var registers = GetRegisters(harness.Machine);

        Assert.Equal(0x10, registers[0]);
        Assert.Equal(0x0F, registers[1]);
        Assert.Equal(0xF1, registers[2]);
        Assert.Equal(0, registers[0xF]);
    }

    [Fact]
    public void DrawSpriteXorAndCollisionFlagWorks()
    {
        var harness = new Chip8MachineHarness();
        harness.LoadProgram(
            0x60, 0x00,
            0x61, 0x00,
            0xA3, 0x10,
            0xD0, 0x11,
            0xD0, 0x11
        );

        // Install sprite at I=0x310 manually
        var sprite = new byte[] { 0xF0 };
        InjectSprite(harness.Machine, 0x310, sprite);

        harness.Run(4); // draw once
        var intermediate = new byte[Chip8Machine.DisplayWidth * Chip8Machine.DisplayHeight];
        harness.Machine.CopyDisplayTo(intermediate);
        Assert.Contains((byte)1, intermediate);

        harness.Run(1); // draw again to toggle pixels off and set collision flag
        var registers = GetRegisters(harness.Machine);
        Assert.Equal(1, registers[0xF]);
        var buffer = new byte[Chip8Machine.DisplayWidth * Chip8Machine.DisplayHeight];
        harness.Machine.CopyDisplayTo(buffer);
        Assert.DoesNotContain((byte)1, buffer);
    }

    [Fact]
    public void MemoryTransferPersistsRegisters()
    {
        var harness = new Chip8MachineHarness();
        harness.LoadProgram(
            0x60, 0x0A,
            0x61, 0x14,
            0xA3, 0x20,
            0xF1, 0x55,
            0x60, 0x00,
            0x61, 0x00,
            0xA3, 0x20,
            0xF1, 0x65
        );

        harness.Run(8);
        var registers = GetRegisters(harness.Machine);
        Assert.Equal(0x0A, registers[0]);
        Assert.Equal(0x14, registers[1]);
    }

    [Fact]
    public void BcdWritesDigitsToMemory()
    {
        var harness = new Chip8MachineHarness();
        harness.LoadProgram(
            0x60, 0x7B,
            0xA3, 0x30,
            0xF0, 0x33
        );

        harness.Run(3);
        var memory = GetMemory(harness.Machine);
        Assert.Equal(0x01, memory[0x330]);
        Assert.Equal(0x02, memory[0x331]);
        Assert.Equal(0x03, memory[0x332]);
    }

    private static byte[] GetRegisters(Chip8Machine machine)
    {
        var field = typeof(Chip8Machine).GetField("_registers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (byte[])field!.GetValue(machine)!;
    }

    private static byte[] GetMemory(Chip8Machine machine)
    {
        var field = typeof(Chip8Machine).GetField("_memory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (byte[])field!.GetValue(machine)!;
    }

    private static void InjectSprite(Chip8Machine machine, int address, byte[] sprite)
    {
        var memory = GetMemory(machine);
        Array.Copy(sprite, 0, memory, address, sprite.Length);
        var indexField = typeof(Chip8Machine).GetField("_indexRegister", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        indexField!.SetValue(machine, (ushort)address);
    }
}
