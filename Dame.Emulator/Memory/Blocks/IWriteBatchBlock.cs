using System;

namespace Dame.Emulator.Memory.Blocks
{
    public interface IWriteBatchBlock
    {
        void Write(int address, Span<byte> data);
    }
}