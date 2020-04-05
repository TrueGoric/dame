using System;

namespace Dame.Memory.Blocks
{
    interface IReadBlock<T>
        where T : struct
    {
        T Read(int address);
    }
}