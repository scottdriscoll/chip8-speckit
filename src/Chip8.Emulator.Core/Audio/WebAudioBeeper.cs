using System.Threading.Tasks;
using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Timing;

namespace Chip8.Emulator.Audio;

public interface IWebAudioInterop
{
    ValueTask StartBeepAsync(float frequency);
    ValueTask StopBeepAsync();
}

public sealed class WebAudioBeeper
{
    private readonly IWebAudioInterop _interop;
    private readonly EmulatorSnapshotPublisher _snapshotPublisher;
    private readonly float _frequency;
    private bool _isActive;
    private bool _isMuted;

    public WebAudioBeeper(IWebAudioInterop interop, EmulatorSnapshotPublisher snapshotPublisher, float frequency = 440f)
    {
        _interop = interop;
        _snapshotPublisher = snapshotPublisher;
        _frequency = frequency;
        _snapshotPublisher.UpdateAudioChannel(AudioChannelState.Inactive(frequency));
    }

    public bool IsMuted => _isMuted;
    public bool IsActive => _isActive;

    public void Attach(TimerService timerService)
    {
        timerService.SoundTimerUpdated += (_, args) =>
        {
            _ = UpdateSoundTimerAsync(args.SoundTimer, args.CycleNumber);
        };
    }

    public async Task UpdateSoundTimerAsync(byte soundTimer, ulong cycleNumber)
    {
        if (_isMuted)
        {
            if (_isActive)
            {
                await StopAsync().ConfigureAwait(false);
            }

            _snapshotPublisher.UpdateAudioChannel(AudioChannelState.Muted(_frequency));
            return;
        }

        if (soundTimer > 0 && !_isActive)
        {
            await StartAsync(cycleNumber, soundTimer).ConfigureAwait(false);
        }
        else if (soundTimer == 0 && _isActive)
        {
            await StopAsync().ConfigureAwait(false);
        }
    }

    public void ToggleMute(bool isMuted)
    {
        _isMuted = isMuted;
        var state = isMuted
            ? AudioChannelState.Muted(_frequency)
            : (_isActive ? AudioChannelState.Active(_frequency, null) : AudioChannelState.Inactive(_frequency));
        _snapshotPublisher.UpdateAudioChannel(state);
    }

    private async Task StartAsync(ulong cycleNumber, byte soundTimer)
    {
        await _interop.StartBeepAsync(_frequency).ConfigureAwait(false);
        _isActive = true;
        _snapshotPublisher.UpdateAudioChannel(AudioChannelState.Active(_frequency, cycleNumber + soundTimer));
    }

    private async Task StopAsync()
    {
        await _interop.StopBeepAsync().ConfigureAwait(false);
        _isActive = false;
        _snapshotPublisher.UpdateAudioChannel(AudioChannelState.Inactive(_frequency));
    }
}
