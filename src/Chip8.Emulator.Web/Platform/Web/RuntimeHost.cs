using System.Threading.Tasks;
using Chip8.Emulator.Audio;
using Chip8.Emulator.Core.Diagnostics;
using Microsoft.JSInterop;

namespace Chip8.Emulator.Platform.Web;

public sealed class RuntimeHost : IWebAudioInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly EmulatorSnapshotPublisher _snapshotPublisher;

    public RuntimeHost(IJSRuntime jsRuntime, EmulatorSnapshotPublisher snapshotPublisher)
    {
        _jsRuntime = jsRuntime;
        _snapshotPublisher = snapshotPublisher;
        _snapshotPublisher.SnapshotCreated += OnSnapshotCreated;
    }

    public ValueTask InitializeAudioAsync()
    {
        return _jsRuntime.InvokeVoidAsync("eval", "typeof chip8AudioInterop === 'undefined' && console.warn('Audio interop not loaded');");
    }

    public async ValueTask StartBeepAsync(float frequency)
    {
        await _jsRuntime.InvokeVoidAsync("chip8AudioInterop.startBeep", frequency);
    }

    public async ValueTask StopBeepAsync()
    {
        await _jsRuntime.InvokeVoidAsync("chip8AudioInterop.stopBeep");
    }

    public async ValueTask RegisterDebugHooksAsync()
    {
        var snapshot = _snapshotPublisher.CurrentSnapshot;
        await _jsRuntime.InvokeVoidAsync("chip8DebugSetSnapshot", snapshot);
    }

    public ValueTask RenderDisplayAsync(byte[] pixels, int width, int height)
    {
        return _jsRuntime.InvokeVoidAsync("chip8RenderDisplay", pixels, width, height);
    }

    private void OnSnapshotCreated(object? sender, EmulatorStateSnapshot snapshot)
    {
        _ = _jsRuntime.InvokeVoidAsync("chip8DebugSetSnapshot", snapshot);
    }
}
