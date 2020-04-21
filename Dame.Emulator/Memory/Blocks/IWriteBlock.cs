using System;

namespace Dame.Memory.Blocks
{
    public interface IWriteBlock
    {        
        void Write(int address, byte value);
    }
}