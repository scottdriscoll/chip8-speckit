using System;
using Chip8.Emulator.Core.Diagnostics;

namespace Chip8.Emulator.Core.Control;

public sealed class PauseController
{
    private readonly EmulatorSnapshotPublisher _snapshotPublisher;
    private readonly object _gate = new();
    private ExecutionState _state = ExecutionState.Running;

    public PauseController(EmulatorSnapshotPublisher snapshotPublisher)
    {
        _snapshotPublisher = snapshotPublisher;
    }

    public ExecutionState State
    {
        get
        {
            lock (_gate)
            {
                return _state;
            }
        }
    }

    public event EventHandler<ExecutionState>? ExecutionStateChanged;

    public bool Pause()
    {
        lock (_gate)
        {
            if (_state == ExecutionState.Paused)
            {
                return false;
            }

            _state = ExecutionState.Paused;
            PublishState();
            return true;
        }
    }

    public bool Resume()
    {
        lock (_gate)
        {
            if (_state == ExecutionState.Running)
            {
                return false;
            }

            _state = ExecutionState.Running;
            PublishState();
            return true;
        }
    }

    public bool Step(Action executeInstruction)
    {
        lock (_gate)
        {
            if (_state != ExecutionState.Paused)
            {
                return false;
            }

            _state = ExecutionState.Stepping;
            PublishState();
        }

        executeInstruction();

        lock (_gate)
        {
            _state = ExecutionState.Paused;
            PublishState();
        }

        return true;
    }

    private void PublishState()
    {
        _snapshotPublisher.UpdateExecutionState(_state);
        var interaction = new HudInteractionSnapshot
        {
            PauseEnabled = _state != ExecutionState.Paused,
            StepPending = _state == ExecutionState.Stepping
        };
        _snapshotPublisher.UpdateInteraction(interaction);
        ExecutionStateChanged?.Invoke(this, _state);
    }
}
