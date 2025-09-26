using System;
using System.Buffers;
using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Input;
using Chip8.Emulator.Core.Timing;

namespace Chip8.Emulator.Core.Machine;

public sealed class Chip8Machine
{
    public const int DisplayWidth = 64;
    public const int DisplayHeight = 32;
    private const int MemorySize = 4096;
    private const int StartAddress = 0x200;
    private const int FontsetStartAddress = 0x050;
    private const int StackSize = 16;

    public static int ProgramStart => StartAddress;

    private readonly EmulatorSnapshotPublisher _snapshotPublisher;
    private readonly TimerService _timerService;
    private readonly byte[] _memory = new byte[MemorySize];
    private readonly byte[] _registers = new byte[16];
    private readonly ushort[] _stack = new ushort[StackSize];
    private readonly byte[] _display = new byte[DisplayWidth * DisplayHeight];
    private readonly Random _random = new();
    private ushort _indexRegister;
    private ushort _programCounter;
    private byte _stackPointer;
    private bool _waitingForKey;
    private byte _waitingRegister;
    private bool _romLoaded;

    public Chip8Machine(EmulatorSnapshotPublisher snapshotPublisher, TimerService timerService)
    {
        _snapshotPublisher = snapshotPublisher;
        _timerService = timerService;
        Reset();
    }

    public bool HasProgram => _romLoaded;

    public ReadOnlySpan<byte> DisplayBuffer => _display;
    public ushort ProgramCounter => _programCounter;

    public void CopyDisplayTo(Span<byte> destination) => _display.AsSpan().CopyTo(destination);

    public void Reset()
    {
        Array.Clear(_memory);
        Array.Clear(_registers);
        Array.Clear(_stack);
        Array.Clear(_display);
        _indexRegister = 0;
        _programCounter = StartAddress;
        _stackPointer = 0;
        _waitingForKey = false;
        _waitingRegister = 0;
        _romLoaded = false;

        Span<byte> fontset = stackalloc byte[]
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        fontset.CopyTo(_memory.AsSpan(FontsetStartAddress));
        _timerService.LoadTimers(0, 0);
        _snapshotPublisher.UpdateRegisters(_registers);
        _snapshotPublisher.UpdateProgramCounter(_programCounter);
        _snapshotPublisher.UpdateIndexRegister(_indexRegister);
        _snapshotPublisher.UpdateStackDepth(_stackPointer);
        _snapshotPublisher.UpdateCurrentOpcode(0);
    }

    public void LoadRom(ReadOnlySpan<byte> rom)
    {
        Reset();
        if (rom.Length > MemorySize - StartAddress)
        {
            throw new ArgumentException("ROM is too large for Chip-8 memory", nameof(rom));
        }

        rom.CopyTo(_memory.AsSpan(StartAddress));
        _romLoaded = true;
    }

    public void LoadDemoProgram()
    {
        Reset();

        Span<byte> sprite = stackalloc byte[]
        {
            0x3C, // ####
            0x42,
            0xA9,
            0x85,
            0x42,
            0x3C
        };

        var spriteAddress = 0x300;
        sprite.CopyTo(_memory.AsSpan(spriteAddress));

        Span<byte> program = stackalloc byte[]
        {
            0x00, 0xE0,       // CLS
            0x60, 0x10,       // V0 = 0x10
            0x61, 0x08,       // V1 = 0x08
            0xA3, 0x00,       // I = spriteAddress
            0xD0, 0x16,       // Draw 6-byte sprite at (V0,V1)
            0x70, 0x01,       // V0 += 1
            0x71, 0x00,       // V1 += 0
            0x30, 0x30,       // Skip next if V0 == 0x30
            0x12, 0x06,       // Loop back to draw instruction
            0x60, 0x10,       // Reset V0
            0x12, 0x04        // Jump to draw instruction
        };

        program.CopyTo(_memory.AsSpan(StartAddress));
        _romLoaded = true;
    }

    public void SetKeyState(byte nibble, bool pressed)
    {
        if (nibble > 0xF)
        {
            return;
        }

        // When waiting for key press, capture the first press.
        if (_waitingForKey && pressed)
        {
            _registers[_waitingRegister] = nibble;
            _waitingForKey = false;
        }
    }

    public void ExecuteCycles(bool[] keypad, int cycles)
    {
        if (!_romLoaded)
        {
            return;
        }

        for (var i = 0; i < cycles; i++)
        {
            ExecuteCycle(keypad);
        }
    }

    private void ExecuteCycle(bool[] keypad)
    {
        if (_waitingForKey)
        {
            return;
        }

        var opcode = FetchOpcode();
        DecodeAndExecute(opcode, keypad);

        _snapshotPublisher.UpdateRegisters(_registers);
        _snapshotPublisher.UpdateProgramCounter(_programCounter);
        _snapshotPublisher.UpdateIndexRegister(_indexRegister);
        _snapshotPublisher.UpdateStackDepth(_stackPointer);
        _snapshotPublisher.UpdateCurrentOpcode(opcode);
    }

    private ushort FetchOpcode()
    {
        if (_programCounter >= MemorySize - 1)
        {
            _programCounter = (ushort)(_programCounter % (MemorySize - 1));
        }

        var opcode = (ushort)((_memory[_programCounter] << 8) | _memory[_programCounter + 1]);
        _programCounter += 2;
        return opcode;
    }

    private void DecodeAndExecute(ushort opcode, bool[] keypad)
    {
        var nnn = (ushort)(opcode & 0x0FFF);
        var kk = (byte)(opcode & 0x00FF);
        var n = (byte)(opcode & 0x000F);
        var x = (byte)((opcode & 0x0F00) >> 8);
        var y = (byte)((opcode & 0x00F0) >> 4);

        switch (opcode & 0xF000)
        {
            case 0x0000:
                switch (opcode)
                {
                    case 0x00E0:
                        Array.Clear(_display);
                        break;
                    case 0x00EE:
                        if (_stackPointer > 0)
                        {
                            _stackPointer--;
                            _programCounter = _stack[_stackPointer];
                        }
                        break;
                    default:
                        // 0x0NNN is ignored (legacy).
                        break;
                }
                break;
            case 0x1000:
                _programCounter = nnn;
                break;
            case 0x2000:
                _stack[_stackPointer] = _programCounter;
                _stackPointer++;
                _programCounter = nnn;
                break;
            case 0x3000:
                if (_registers[x] == kk)
                {
                    _programCounter += 2;
                }
                break;
            case 0x4000:
                if (_registers[x] != kk)
                {
                    _programCounter += 2;
                }
                break;
            case 0x5000:
                if (n == 0 && _registers[x] == _registers[y])
                {
                    _programCounter += 2;
                }
                break;
            case 0x6000:
                _registers[x] = kk;
                break;
            case 0x7000:
                _registers[x] = (byte)((_registers[x] + kk) & 0xFF);
                break;
            case 0x8000:
                switch (n)
                {
                    case 0x0:
                        _registers[x] = _registers[y];
                        break;
                    case 0x1:
                        _registers[x] |= _registers[y];
                        _registers[0xF] = 0;
                        break;
                    case 0x2:
                        _registers[x] &= _registers[y];
                        _registers[0xF] = 0;
                        break;
                    case 0x3:
                        _registers[x] ^= _registers[y];
                        _registers[0xF] = 0;
                        break;
                    case 0x4:
                    {
                        var sum = _registers[x] + _registers[y];
                        _registers[0xF] = (byte)(sum > 0xFF ? 1 : 0);
                        _registers[x] = (byte)sum;
                        break;
                    }
                    case 0x5:
                    {
                        _registers[0xF] = (byte)(_registers[x] >= _registers[y] ? 1 : 0);
                        _registers[x] = (byte)(_registers[x] - _registers[y]);
                        break;
                    }
                    case 0x6:
                    {
                        _registers[0xF] = (byte)(_registers[x] & 0x1);
                        _registers[x] >>= 1;
                        break;
                    }
                    case 0x7:
                    {
                        _registers[0xF] = (byte)(_registers[y] >= _registers[x] ? 1 : 0);
                        _registers[x] = (byte)(_registers[y] - _registers[x]);
                        break;
                    }
                    case 0xE:
                    {
                        _registers[0xF] = (byte)((_registers[x] & 0x80) >> 7);
                        _registers[x] = (byte)((_registers[x] << 1) & 0xFF);
                        break;
                    }
                }
                break;
            case 0x9000:
                if (n == 0 && _registers[x] != _registers[y])
                {
                    _programCounter += 2;
                }
                break;
            case 0xA000:
                _indexRegister = nnn;
                break;
            case 0xB000:
                _programCounter = (ushort)(nnn + _registers[0]);
                break;
            case 0xC000:
                _registers[x] = (byte)(_random.Next(0, 256) & kk);
                break;
            case 0xD000:
                DrawSprite(_registers[x], _registers[y], n);
                break;
            case 0xE000:
                switch (kk)
                {
                    case 0x9E:
                        if (IsKeyPressed(keypad, _registers[x]))
                        {
                            _programCounter += 2;
                        }
                        break;
                    case 0xA1:
                        if (!IsKeyPressed(keypad, _registers[x]))
                        {
                            _programCounter += 2;
                        }
                        break;
                }
                break;
            case 0xF000:
                switch (kk)
                {
                    case 0x07:
                        _registers[x] = _timerService.DelayTimer;
                        break;
                    case 0x0A:
                        _waitingForKey = true;
                        _waitingRegister = x;
                        _programCounter -= 2; // repeat until key pressed
                        break;
                    case 0x15:
                        _timerService.SetDelayTimer(_registers[x]);
                        break;
                    case 0x18:
                        _timerService.SetSoundTimer(_registers[x]);
                        break;
                    case 0x1E:
                    {
                        var result = _indexRegister + _registers[x];
                        _registers[0xF] = (byte)(result > 0x0FFF ? 1 : 0);
                        _indexRegister = (ushort)(result & 0x0FFF);
                        break;
                    }
                    case 0x29:
                        _indexRegister = (ushort)(FontsetStartAddress + (_registers[x] * 5));
                        break;
                    case 0x33:
                    {
                        var value = _registers[x];
                        _memory[_indexRegister] = (byte)(value / 100);
                        _memory[_indexRegister + 1] = (byte)((value / 10) % 10);
                        _memory[_indexRegister + 2] = (byte)(value % 10);
                        break;
                    }
                    case 0x55:
                        CopyRegistersToMemory(x);
                        _indexRegister = (ushort)((_indexRegister + x + 1) & 0x0FFF);
                        break;
                    case 0x65:
                        ReadRegistersFromMemory(x);
                        _indexRegister = (ushort)((_indexRegister + x + 1) & 0x0FFF);
                        break;
                }
                break;
            default:
                // Unknown opcode – ignore to keep emulator running
                break;
        }
    }

    private void DrawSprite(byte x, byte y, byte height)
    {
        _registers[0xF] = 0;
        for (var row = 0; row < height; row++)
        {
            var spriteByte = _memory[_indexRegister + row];
            for (var col = 0; col < 8; col++)
            {
                if ((spriteByte & (0x80 >> col)) == 0)
                {
                    continue;
                }

                var px = (x + col) % DisplayWidth;
                var py = (y + row) % DisplayHeight;
                var index = py * DisplayWidth + px;
                if (_display[index] == 1)
                {
                    _registers[0xF] = 1;
                }

                _display[index] ^= 1;
            }
        }
    }

    private bool IsKeyPressed(bool[] keypad, byte key)
    {
        if (key > 0xF)
        {
            return false;
        }

        return keypad.Length > key && keypad[key];
    }

    private void CopyRegistersToMemory(byte upToRegister)
    {
        for (byte i = 0; i <= upToRegister; i++)
        {
            var address = _indexRegister + i;
            if (address >= MemorySize)
            {
                break;
            }

            _memory[address] = _registers[i];
        }
    }

    private void ReadRegistersFromMemory(byte upToRegister)
    {
        for (byte i = 0; i <= upToRegister; i++)
        {
            var address = _indexRegister + i;
            if (address >= MemorySize)
            {
                break;
            }

            _registers[i] = _memory[address];
        }
    }
}
