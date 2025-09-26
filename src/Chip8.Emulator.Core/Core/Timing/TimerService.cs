using System;
using Chip8.Emulator.Core.Diagnostics;

namespace Chip8.Emulator.Core.Timing;

public sealed class TimerService
{
    private const double TimerFrequency = 60.0;
    private readonly TimeSpan _tickDuration = TimeSpan.FromSeconds(1 / TimerFrequency);
    private readonly EmulatorSnapshotPublisher _snapshotPublisher;
    private TimeSpan _accumulator = TimeSpan.Zero;

    public TimerService(EmulatorSnapshotPublisher snapshotPublisher)
    {
        _snapshotPublisher = snapshotPublisher;
    }

    public byte DelayTimer { get; private set; }
    public byte SoundTimer { get; private set; }

    public event EventHandler<SoundTimerChangedEventArgs>? SoundTimerUpdated;

    public void LoadTimers(byte delayTimer, byte soundTimer)
    {
        DelayTimer = delayTimer;
        SoundTimer = soundTimer;
        _snapshotPublisher.UpdateTimers(DelayTimer, SoundTimer);
        SoundTimerUpdated?.Invoke(this, new SoundTimerChangedEventArgs(SoundTimer, _snapshotPublisher.CurrentSnapshot.CycleNumber));
    }

    public void SetDelayTimer(byte value)
    {
        DelayTimer = value;
        _snapshotPublisher.UpdateTimers(DelayTimer, SoundTimer);
    }

    public void SetSoundTimer(byte value)
    {
        SoundTimer = value;
        _snapshotPublisher.UpdateTimers(DelayTimer, SoundTimer);
        SoundTimerUpdated?.Invoke(this, new SoundTimerChangedEventArgs(SoundTimer, _snapshotPublisher.CurrentSnapshot.CycleNumber));
    }

    public void Advance(TimeSpan elapsed)
    {
        _accumulator += elapsed;
        while (_accumulator >= _tickDuration)
        {
            _accumulator -= _tickDuration;
            Tick();
        }
    }

    public void ForceTick()
    {
        Tick();
    }

    private void Tick()
    {
        if (DelayTimer > 0)
        {
            DelayTimer--;
        }

        var soundChanged = false;
        if (SoundTimer > 0)
        {
            SoundTimer--;
            soundChanged = true;
        }

        _snapshotPublisher.UpdateTimers(DelayTimer, SoundTimer);
        _snapshotPublisher.PublishSnapshot();

        if (soundChanged)
        {
            var cycleNumber = _snapshotPublisher.CurrentSnapshot.CycleNumber;
            SoundTimerUpdated?.Invoke(this, new SoundTimerChangedEventArgs(SoundTimer, cycleNumber));
        }
    }
}

public sealed record SoundTimerChangedEventArgs(byte SoundTimer, ulong CycleNumber);
