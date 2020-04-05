using System;

namespace Dame.Memory.Blocks
{
    interface IWriteBlock<T>
        where T : unmanaged
    {        
        void Write(int address, T value);
    }
}