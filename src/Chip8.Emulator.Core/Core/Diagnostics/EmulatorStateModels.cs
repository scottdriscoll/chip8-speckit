using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Chip8.Emulator.Core.Diagnostics;

public enum ExecutionState
{
    Running,
    Paused,
    Stepping
}

public sealed class EmulatorStateSnapshot
{
    public EmulatorStateSnapshot()
    {
        Registers = new byte[16];
        Panels = Array.Empty<HudPanelDescriptor>();
        KeyboardLayout = KeyboardLayoutProfile.Empty;
        AudioChannel = AudioChannelState.Muted();
        Interaction = new HudInteractionSnapshot();
    }

    public byte[] Registers { get; set; }
    public ushort IndexRegister { get; set; }
    public ushort ProgramCounter { get; set; }
    public byte StackDepth { get; set; }
    public byte DelayTimer { get; set; }
    public byte SoundTimer { get; set; }
    public ushort CurrentOpcode { get; set; }
    public ulong CycleNumber { get; set; }
    public ExecutionState ExecutionState { get; set; } = ExecutionState.Running;
    public KeyboardLayoutProfile KeyboardLayout { get; set; }
    public AudioChannelState AudioChannel { get; set; }
    public IReadOnlyList<HudPanelDescriptor> Panels { get; set; }
    public HudInteractionSnapshot Interaction { get; set; }

    public EmulatorStateSnapshot Clone()
    {
        return new EmulatorStateSnapshot
        {
            Registers = (byte[])Registers.Clone(),
            IndexRegister = IndexRegister,
            ProgramCounter = ProgramCounter,
            StackDepth = StackDepth,
            DelayTimer = DelayTimer,
            SoundTimer = SoundTimer,
            CurrentOpcode = CurrentOpcode,
            CycleNumber = CycleNumber,
            ExecutionState = ExecutionState,
            KeyboardLayout = KeyboardLayout.Clone(),
            AudioChannel = AudioChannel.Clone(),
            Panels = Panels.Select(p => p.Clone()).ToArray(),
            Interaction = Interaction.Clone()
        };
    }
}

public sealed class HudPanelDescriptor
{
    public HudPanelDescriptor(
        string panelId,
        string title,
        IReadOnlyList<HudValueDescriptor> values,
        HudPanelLayout layout,
        TimeSpan updateCadence)
    {
        PanelId = panelId;
        Title = title;
        Values = values;
        Layout = layout;
        UpdateCadence = updateCadence;
    }

    public string PanelId { get; }
    public string Title { get; }
    public IReadOnlyList<HudValueDescriptor> Values { get; }
    public HudPanelLayout Layout { get; }
    public TimeSpan UpdateCadence { get; }

    public HudPanelDescriptor Clone()
    {
        return new HudPanelDescriptor(
            PanelId,
            Title,
            Values.Select(v => v.Clone()).ToArray(),
            Layout,
            UpdateCadence);
    }
}

public sealed class HudValueDescriptor
{
    public HudValueDescriptor(string label, string value, bool highlight)
    {
        Label = label;
        Value = value;
        Highlight = highlight;
    }

    public string Label { get; }
    public string Value { get; }
    public bool Highlight { get; }

    public HudValueDescriptor Clone() => new(Label, Value, Highlight);
}

public sealed record HudPanelLayout(int Row, int Column, int RowSpan = 1, int ColumnSpan = 1);

public sealed class KeyboardLayoutProfile
{
    public static KeyboardLayoutProfile Empty { get; } = new(
        ImmutableDictionary<byte, string>.Empty,
        ImmutableDictionary<byte, string>.Empty,
        ImmutableDictionary<byte, bool>.Empty,
        null);

    public KeyboardLayoutProfile(
        IReadOnlyDictionary<byte, string> mapping,
        IReadOnlyDictionary<byte, string> displayLabels,
        IReadOnlyDictionary<byte, bool> highlightStates,
        string? fallbackGuidance)
    {
        Mapping = mapping;
        DisplayLabels = displayLabels;
        HighlightStates = highlightStates;
        FallbackGuidance = fallbackGuidance;
    }

    public IReadOnlyDictionary<byte, string> Mapping { get; }
    public IReadOnlyDictionary<byte, string> DisplayLabels { get; }
    public IReadOnlyDictionary<byte, bool> HighlightStates { get; }
    public string? FallbackGuidance { get; }

    public KeyboardLayoutProfile Clone()
    {
        return new KeyboardLayoutProfile(
            Mapping.ToImmutableDictionary(),
            DisplayLabels.ToImmutableDictionary(),
            HighlightStates.ToImmutableDictionary(),
            FallbackGuidance);
    }
}

public sealed class AudioChannelState
{
    private AudioChannelState(bool isMuted, bool isActive, float frequency, ulong? nextStopTick)
    {
        IsMuted = isMuted;
        IsActive = isActive;
        Frequency = frequency;
        NextStopTick = nextStopTick;
    }

    public bool IsMuted { get; }
    public bool IsActive { get; }
    public float Frequency { get; }
    public ulong? NextStopTick { get; }

    public static AudioChannelState Muted(float frequency = 440f) => new(true, false, frequency, null);

    public static AudioChannelState Active(float frequency, ulong? nextStopTick) => new(false, true, frequency, nextStopTick);

    public static AudioChannelState Inactive(float frequency) => new(false, false, frequency, null);

    public AudioChannelState WithMuted(bool muted)
        => new(muted, muted ? false : IsActive, Frequency, muted ? null : NextStopTick);

    public AudioChannelState WithActive(bool active, ulong? nextStopTick)
        => new(IsMuted, active, Frequency, nextStopTick);

    public AudioChannelState Clone() => new(IsMuted, IsActive, Frequency, NextStopTick);
}

public sealed class HudInteractionSnapshot
{
    public bool PauseEnabled { get; set; } = true;
    public bool StepPending { get; set; }
    public string? ErrorMessage { get; set; }
    public Queue<string> Notifications { get; set; } = new();

    public HudInteractionSnapshot Clone()
    {
        return new HudInteractionSnapshot
        {
            PauseEnabled = PauseEnabled,
            StepPending = StepPending,
            ErrorMessage = ErrorMessage,
            Notifications = new Queue<string>(Notifications)
        };
    }
}
