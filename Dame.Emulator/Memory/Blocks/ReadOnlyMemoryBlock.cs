using System;
using System.Runtime.CompilerServices;

namespace Dame.Emulator.Memory.Blocks
{
    public sealed class ReadOnlyMemoryBlock : IReadBlock
    {
        private byte[] memory;

        public byte[] Memory => memory;

        public ReadOnlyMemoryBlock(int size)
        {
            this.memory = new byte[size];
        }

        public byte Read(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            return memory[address];
        }
    }
}