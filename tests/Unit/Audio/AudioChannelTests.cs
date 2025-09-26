using System.Threading.Tasks;
using Chip8.Emulator.Audio;
using Chip8.Emulator.Core.Diagnostics;
using Xunit;

namespace Chip8.Emulator.UnitTests.Audio;

public class AudioChannelTests
{
    [Fact]
    public async Task Should_start_beep_when_sound_timer_transitions_positive()
    {
        var fake = new FakeWebAudioInterop();
        var publisher = new EmulatorSnapshotPublisher();
        var beeper = new WebAudioBeeper(fake, publisher);

        await beeper.UpdateSoundTimerAsync(5, cycleNumber: 10);

        Assert.Equal(1, fake.StartCalls);
        Assert.True(publisher.CurrentSnapshot.AudioChannel.IsActive);
    }

    [Fact]
    public async Task Should_stop_beep_when_sound_timer_hits_zero()
    {
        var fake = new FakeWebAudioInterop();
        var publisher = new EmulatorSnapshotPublisher();
        var beeper = new WebAudioBeeper(fake, publisher);

        await beeper.UpdateSoundTimerAsync(4, cycleNumber: 10);
        await beeper.UpdateSoundTimerAsync(0, cycleNumber: 15);

        Assert.Equal(1, fake.StartCalls);
        Assert.Equal(1, fake.StopCalls);
        Assert.False(publisher.CurrentSnapshot.AudioChannel.IsActive);
    }

    [Fact]
    public async Task Should_not_start_beep_when_muted()
    {
        var fake = new FakeWebAudioInterop();
        var publisher = new EmulatorSnapshotPublisher();
        var beeper = new WebAudioBeeper(fake, publisher);

        beeper.ToggleMute(true);
        await beeper.UpdateSoundTimerAsync(3, cycleNumber: 5);

        Assert.Equal(0, fake.StartCalls);
        Assert.True(publisher.CurrentSnapshot.AudioChannel.IsMuted);
    }

    private sealed class FakeWebAudioInterop : IWebAudioInterop
    {
        public int StartCalls { get; private set; }
        public int StopCalls { get; private set; }

        public ValueTask StartBeepAsync(float frequency)
        {
            StartCalls++;
            return ValueTask.CompletedTask;
        }

        public ValueTask StopBeepAsync()
        {
            StopCalls++;
            return ValueTask.CompletedTask;
        }
    }
}
