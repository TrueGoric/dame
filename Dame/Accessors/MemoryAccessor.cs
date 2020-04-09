using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dame.Memory;

namespace Dame.Accessors
{
    sealed class MemoryAccessor
    {
        private readonly MemoryController memoryController;

        public MemoryAccessor(MemoryController controller)
        {
            memoryController = controller;
        }

        public int Location { get; set; }

        public byte Read() => memoryController.Read(Location++);

        public unsafe ushort ReadDouble()
        {
            var value = memoryController.ReadDouble(Location);

            Location += 2;

            return value;
        }
    }
}