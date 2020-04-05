using System;

namespace Dame.Memory.Blocks
{
    interface IReadBlock<T>
        where T : unmanaged
    {
        T Read(int address);
    }
}