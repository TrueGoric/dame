using System;

namespace Dame.Memory.Blocks
{
    public interface IModifyBlock : IReadBlock, IWriteBlock
    {
        ref byte Get(int address);
    }
}