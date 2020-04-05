using System;
using System.Runtime.CompilerServices;

namespace Dame.Accessors
{
    sealed class MemoryAccessor<T>
        where T : struct
    {
        private readonly Memory<T> memory;

        public int Location { get; private set; }

        public MemoryAccessor(Memory<T> memory)
        {
            this.memory = memory;
        }

        public T Read() => ReadRef();

        public ref T ReadRef()
        {
            if (Location >= memory.Length)
                throw new InvalidOperationException($"Address {Location} is out of bounds!");
            
            return ref ReadRefAtInternal(Location++);
        }

        public T ReadAt(int address) => ReadRefAt(address);

        public ref T ReadRefAt(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));
            
            return ref ReadRefAtInternal(address);
        }

        public void WriteAt(int address, T value)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            memory.Span[address] = value;
        }

        public void JumpTo(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            Location = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref T ReadRefAtInternal(int address)
            => ref memory.Span[address];
    }
}