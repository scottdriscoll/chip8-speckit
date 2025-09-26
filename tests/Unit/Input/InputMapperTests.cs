using System;
using System.Linq;
using Chip8.Emulator.Core.Input;
using Xunit;

namespace Chip8.Emulator.UnitTests.Input;

public class InputMapperTests
{
    [Fact]
    public void Should_map_all_chip8_keys_to_keyboard_codes()
    {
        var mapper = new InputMapper();

        var mapping = mapper.GetMapping();

        Assert.Equal(16, mapping.Count);
        Assert.Contains(mapping, kvp => kvp.Key == 0x1 && kvp.Value == "Digit1");
        Assert.Contains(mapping, kvp => kvp.Key == 0xF && kvp.Value == "KeyV");
        Assert.Equal(mapping.Keys.OrderBy(k => k), Enumerable.Range(0, 16).Select(Chip8Order).OrderBy(k => k));

        static byte Chip8Order(int index) => index switch
        {
            0 => 0x1,
            1 => 0x2,
            2 => 0x3,
            3 => 0xC,
            4 => 0x4,
            5 => 0x5,
            6 => 0x6,
            7 => 0xD,
            8 => 0x7,
            9 => 0x8,
            10 => 0x9,
            11 => 0xE,
            12 => 0xA,
            13 => 0x0,
            14 => 0xB,
            15 => 0xF,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    [Fact]
    public void Should_publish_keypad_highlight_states_each_tick()
    {
        var mapper = new InputMapper();

        mapper.HandleKeyDown("Digit1");
        mapper.HandleKeyDown("KeyQ");

        var profile = mapper.BuildProfile();

        Assert.True(profile.HighlightStates[0x1]);
        Assert.True(profile.HighlightStates[0x4]);

        mapper.HandleKeyUp("Digit1");
        profile = mapper.BuildProfile();

        Assert.False(profile.HighlightStates[0x1]);
        Assert.True(profile.HighlightStates[0x4]);
    }

    [Fact]
    public void Should_report_fallback_guidance_for_unknown_keys()
    {
        var mapper = new InputMapper();

        mapper.HandleKeyDown("KeyP");
        var profile = mapper.BuildProfile();

        Assert.NotNull(profile.FallbackGuidance);
        Assert.Contains("Key 'KeyP'", profile.FallbackGuidance);

        // Guidance resets after snapshot so subsequent snapshots don't repeat the message.
        profile = mapper.BuildProfile();
        Assert.Null(profile.FallbackGuidance);
    }
}
