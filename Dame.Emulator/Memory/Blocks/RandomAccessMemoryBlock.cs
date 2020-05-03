using System;

namespace Dame.Emulator.Memory.Blocks
{
    public sealed class RandomAccessMemoryBlock : IModifyBlock
    {
        private byte[] memory;

        public byte[] Memory => memory;

        public RandomAccessMemoryBlock(int size)
        {
            this.memory = new byte[size];
        }

        public ref byte Get(int address)
        {
            ThrowIfOutOfBounds(address);

            return ref memory[address];
        }

        public byte Read(int address)
        {
            ThrowIfOutOfBounds(address);

            return memory[address];
        }

        public void Write(int address, byte value)
        {
            ThrowIfOutOfBounds(address);

            memory[address] = value;
        }

        private void ThrowIfOutOfBounds(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));
        }
    }
}