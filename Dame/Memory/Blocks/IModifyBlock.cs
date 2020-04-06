using System;

namespace Dame.Memory.Blocks
{
    interface IModifyBlock : IReadBlock, IWriteBlock
    {
        ref byte Get(int address);
    }
}