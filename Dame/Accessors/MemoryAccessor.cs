using System;

namespace Dame.Accessors
{
    abstract class MemoryAccessor<T>
        where T : struct
    {
        protected readonly Memory<T> Memory;

        public int Location { get; protected set; }

        public MemoryAccessor(Memory<T> memory)
        {
            Memory = memory;
        }

        public T Read() => ReadRef();

        public abstract ref T ReadRef();

        public abstract void WriteAt(int address, T value);

        public virtual void JumpTo(int address)
        {
            if (address >= Memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));

            Location = address;
        }
    }
}