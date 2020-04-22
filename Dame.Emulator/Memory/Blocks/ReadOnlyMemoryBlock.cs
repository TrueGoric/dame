using System;
using System.Runtime.CompilerServices;

namespace Dame.Emulator.Memory.Blocks
{
    public sealed class ReadOnlyMemoryBlock : IReadBlock
    {
        private readonly Memory<byte> memory;

        public ReadOnlyMemoryBlock(Memory<byte> memory)
        {
            this.memory = memory;
        }

        public byte Read(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            return memory.Span[address];
        }
    }
}