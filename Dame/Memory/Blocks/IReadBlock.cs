using System;

namespace Dame.Memory.Blocks
{
    interface IReadBlock
    {
        byte Read(int address);
    }
}