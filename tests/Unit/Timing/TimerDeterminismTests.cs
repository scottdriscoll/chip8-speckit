using System;
using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Timing;
using Xunit;

namespace Chip8.Emulator.UnitTests.Timing;

public class TimerDeterminismTests
{
    [Fact]
    public void Should_emit_delay_and_sound_timer_snapshots_at_60hz()
    {
        var publisher = new EmulatorSnapshotPublisher();
        var timer = new TimerService(publisher);

        timer.LoadTimers(delayTimer: 5, soundTimer: 5);
        timer.Advance(TimeSpan.FromSeconds(1));

        var snapshot = publisher.CurrentSnapshot;
        Assert.Equal(0, timer.DelayTimer);
        Assert.Equal(0, timer.SoundTimer);
        Assert.Equal(0, snapshot.DelayTimer);
        Assert.Equal(0, snapshot.SoundTimer);
        Assert.True(snapshot.CycleNumber >= 5);
    }

    [Fact]
    public void Should_increment_cycle_counter_each_tick()
    {
        var publisher = new EmulatorSnapshotPublisher();
        var timer = new TimerService(publisher);

        var initialCycle = publisher.CurrentSnapshot.CycleNumber;
        timer.ForceTick();
        timer.ForceTick();

        var snapshot = publisher.CurrentSnapshot;
        Assert.Equal(initialCycle + 2, snapshot.CycleNumber);
    }
}
