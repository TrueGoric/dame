using System;

namespace Dame.Memory.Blocks
{
    public interface IReadBlock
    {
        byte Read(int address);
    }
}