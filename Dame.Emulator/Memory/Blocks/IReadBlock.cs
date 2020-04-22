using System;

namespace Dame.Emulator.Memory.Blocks
{
    public interface IReadBlock
    {
        byte Read(int address);
    }
}