using System;
using System.Runtime.CompilerServices;

namespace Dame.Memory.Blocks
{
    sealed class ReadOnlyMemoryBlock<T> : IReadBlock<T>
        where T : struct
    {
        private readonly Memory<T> memory;
        
        public ReadOnlyMemoryBlock(Memory<T> memory)
        {
            this.memory = memory;
        }

        public T Read(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            return memory.Span[address];
        }
    }
}