using System;

namespace Dame.Memory.Blocks
{
    interface IModifyBlock<T> : IReadBlock<T>, IWriteBlock<T>
        where T : unmanaged
    {
        ref T Get(int address);
    }
}