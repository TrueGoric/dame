using System;

namespace Dame.Memory.Blocks
{
    interface IWriteBlock<T>
        where T : struct
    {        
        void Write(int address, T value);
    }
}