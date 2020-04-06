using System;

namespace Dame.Memory.Blocks
{
    sealed class RandomAccessMemoryBlock : IModifyBlock
    {
        private readonly Memory<byte> memory;

        public RandomAccessMemoryBlock(Memory<byte> memory)
        {
            this.memory = memory;
        }

        public ref byte Get(int address)
        {
            ThrowIfOutOfBounds(address);

            return ref memory.Span[address];
        }

        public byte Read(int address)
        {
            ThrowIfOutOfBounds(address);

            return memory.Span[address];
        }

        public void Write(int address, byte value)
        {
            ThrowIfOutOfBounds(address);

            memory.Span[address] = value;
        }

        private void ThrowIfOutOfBounds(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));
        }
    }
}