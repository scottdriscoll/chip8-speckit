using System.IO;
using Xunit;

namespace Chip8.Emulator.RomTests.Compatibility;

public class Chip8CompatibilityTests
{
    private static readonly string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

    [Theory]
    [InlineData("Airplane.ch8")]
    [InlineData("Pong (1 player).ch8")]
    [InlineData("Tetris [Fran Dachille, 1991].ch8")]
    public void RomProducesNonEmptyDisplay(string romName)
    {
        var path = Path.Combine(AssetsPath, romName);
        if (!File.Exists(path))
        {
            Assert.Fail($"Missing ROM file: {path}");
        }

        var rom = File.ReadAllBytes(path);
        var harness = new InstructionHarness();
        harness.LoadAndRun(rom, cycles: 4000);

        var display = harness.SnapshotDisplay();
        Assert.Contains((byte)1, display);
        Assert.NotEqual(Chip8.Emulator.Core.Machine.Chip8Machine.ProgramStart, harness.Machine.ProgramCounter);
    }
}
