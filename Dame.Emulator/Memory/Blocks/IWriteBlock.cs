using System;

namespace Dame.Emulator.Memory.Blocks
{
    public interface IWriteBlock
    {        
        void Write(int address, byte value);
    }
}