using System;

namespace Dame.Memory.Blocks
{
    interface IWriteBlock
    {        
        void Write(int address, byte value);
    }
}