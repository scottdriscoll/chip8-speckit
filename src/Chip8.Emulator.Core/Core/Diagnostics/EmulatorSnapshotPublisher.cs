using System;
using System.Collections.Generic;
using System.Linq;
using Chip8.Emulator.Core.Input;

namespace Chip8.Emulator.Core.Diagnostics;

public sealed class EmulatorSnapshotPublisher
{
    private readonly object _gate = new();
    private readonly EmulatorStateSnapshot _snapshot = new();
    private byte[] _previousRegisters = new byte[16];
    private readonly List<HudPanelDescriptor> _panels = new();

    public event EventHandler<EmulatorStateSnapshot>? SnapshotCreated;

    public void UpdateRegisters(ReadOnlySpan<byte> registers)
    {
        if (registers.Length != 16)
        {
            throw new ArgumentException("Chip-8 requires 16 registers", nameof(registers));
        }

        lock (_gate)
        {
            registers.CopyTo(_snapshot.Registers);
        }
    }

    public void UpdateIndexRegister(ushort value)
    {
        lock (_gate)
        {
            _snapshot.IndexRegister = value;
        }
    }

    public void UpdateProgramCounter(ushort value)
    {
        lock (_gate)
        {
            _snapshot.ProgramCounter = value;
        }
    }

    public void UpdateStackDepth(byte value)
    {
        lock (_gate)
        {
            _snapshot.StackDepth = value;
        }
    }

    public void UpdateTimers(byte delayTimer, byte soundTimer)
    {
        lock (_gate)
        {
            _snapshot.DelayTimer = delayTimer;
            _snapshot.SoundTimer = soundTimer;
        }
    }

    public void UpdateCurrentOpcode(ushort opcode)
    {
        lock (_gate)
        {
            _snapshot.CurrentOpcode = opcode;
        }
    }

    public void UpdateKeyboard(InputMapper inputMapper)
    {
        lock (_gate)
        {
            _snapshot.KeyboardLayout = inputMapper.BuildProfile();
        }
    }

    public void UpdateAudioChannel(AudioChannelState state)
    {
        lock (_gate)
        {
            _snapshot.AudioChannel = state.Clone();
        }
    }

    public void UpdateInteraction(HudInteractionSnapshot interaction)
    {
        lock (_gate)
        {
            _snapshot.Interaction = interaction.Clone();
        }
    }

    public void UpdateExecutionState(ExecutionState state)
    {
        lock (_gate)
        {
            _snapshot.ExecutionState = state;
        }
    }

    public void ConfigurePanels(IEnumerable<HudPanelDescriptor> panels)
    {
        lock (_gate)
        {
            _panels.Clear();
            _panels.AddRange(panels);
            _snapshot.Panels = _panels.Select(p => p.Clone()).ToArray();
        }
    }

    public EmulatorStateSnapshot CurrentSnapshot
    {
        get
        {
            lock (_gate)
            {
                return _snapshot.Clone();
            }
        }
    }

    public void PublishSnapshot()
    {
        lock (_gate)
        {
            _snapshot.CycleNumber++;
            SnapshotCreated?.Invoke(this, _snapshot.Clone());
            Array.Copy(_snapshot.Registers, _previousRegisters, 16);
        }
    }

    public IReadOnlyList<HudValueDescriptor> BuildRegisterDescriptors()
    {
        lock (_gate)
        {
            var descriptors = new List<HudValueDescriptor>(16);
            for (var i = 0; i < 16; i++)
            {
                var registerValue = _snapshot.Registers[i];
                var previousValue = _previousRegisters[i];
                descriptors.Add(new HudValueDescriptor($"V{i:X1}", $"0x{registerValue:X2}", registerValue != previousValue));
            }

            return descriptors;
        }
    }
}
