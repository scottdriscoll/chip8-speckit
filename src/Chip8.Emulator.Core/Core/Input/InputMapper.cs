using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Chip8.Emulator.Core.Diagnostics;

namespace Chip8.Emulator.Core.Input;

public sealed class InputMapper
{
    private static readonly IReadOnlyDictionary<byte, string> DefaultMapping = new Dictionary<byte, string>
    {
        { 0x0, "KeyX" },
        { 0x1, "Digit1" },
        { 0x2, "Digit2" },
        { 0x3, "Digit3" },
        { 0x4, "KeyQ" },
        { 0x5, "KeyW" },
        { 0x6, "KeyE" },
        { 0x7, "KeyA" },
        { 0x8, "KeyS" },
        { 0x9, "KeyD" },
        { 0xA, "KeyZ" },
        { 0xB, "KeyC" },
        { 0xC, "Digit4" },
        { 0xD, "KeyR" },
        { 0xE, "KeyF" },
        { 0xF, "KeyV" }
    };

    private static readonly IReadOnlyDictionary<byte, string> DefaultLabels = new Dictionary<byte, string>
    {
        { 0x0, "X" },
        { 0x1, "1" },
        { 0x2, "2" },
        { 0x3, "3" },
        { 0x4, "Q" },
        { 0x5, "W" },
        { 0x6, "E" },
        { 0x7, "A" },
        { 0x8, "S" },
        { 0x9, "D" },
        { 0xA, "Z" },
        { 0xB, "C" },
        { 0xC, "4" },
        { 0xD, "R" },
        { 0xE, "F" },
        { 0xF, "V" }
    };

    private readonly IReadOnlyDictionary<byte, string> _mapping;
    private readonly IReadOnlyDictionary<string, byte> _reverseLookup;
    private readonly bool[] _highlightStates = new bool[16];
    private readonly object _gate = new();
    private string? _fallbackMessage;

    public InputMapper()
        : this(DefaultMapping)
    {
    }

    public InputMapper(IReadOnlyDictionary<byte, string> mapping)
    {
        _mapping = mapping.ToImmutableDictionary();
        _reverseLookup = _mapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
                                 .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
    }

    public KeyboardLayoutProfile BuildProfile()
    {
        lock (_gate)
        {
            var highlightSnapshot = Enumerable.Range(0, 16)
                .ToDictionary(i => (byte)i, i => _highlightStates[i]);

            var profile = new KeyboardLayoutProfile(
                _mapping,
                DefaultLabels,
                highlightSnapshot,
                _fallbackMessage);

            _fallbackMessage = null;
            return profile;
        }
    }

    public void RegisterFallback(string message)
    {
        lock (_gate)
        {
            _fallbackMessage = message;
        }
    }

    public void HandleKeyDown(string keyCode)
    {
        if (!_reverseLookup.TryGetValue(keyCode, out var nibble))
        {
            RegisterFallback($"Key '{keyCode}' is not part of the Chip-8 layout. Consider remapping your keyboard settings.");
            return;
        }

        lock (_gate)
        {
            _highlightStates[nibble] = true;
        }
    }

    public void HandleKeyUp(string keyCode)
    {
        if (!_reverseLookup.TryGetValue(keyCode, out var nibble))
        {
            return;
        }

        lock (_gate)
        {
            _highlightStates[nibble] = false;
        }
    }

    public IReadOnlyDictionary<byte, string> GetMapping() => _mapping;

    public bool TryResolveKeyCode(string keyCode, out byte nibble)
    {
        return _reverseLookup.TryGetValue(keyCode, out nibble);
    }
}
