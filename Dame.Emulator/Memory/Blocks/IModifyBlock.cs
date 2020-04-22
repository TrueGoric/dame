using System;

namespace Dame.Emulator.Memory.Blocks
{
    public interface IModifyBlock : IReadBlock, IWriteBlock
    {
        ref byte Get(int address);
    }
}