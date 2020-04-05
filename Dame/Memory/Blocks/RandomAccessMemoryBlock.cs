using System;

namespace Dame.Memory.Blocks
{
    sealed class RandomAccessMemoryBlock<T> : IModifyBlock<T>
        where T : struct
    {
        private readonly Memory<T> memory;

        public RandomAccessMemoryBlock(Memory<T> memory)
        {
            this.memory = memory;
        }

        public ref T Get(int address)
        {
            ThrowIfOutOfBounds(address);

            return ref memory.Span[address];
        }

        public T Read(int address)
        {
            ThrowIfOutOfBounds(address);

            return memory.Span[address];
        }

        public void Write(int address, T value)
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