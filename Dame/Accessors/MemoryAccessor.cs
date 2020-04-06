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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read() => memoryController.Read(Location++);

        public unsafe ushort ReadDouble()
        {
            Span<byte> toBeCast = stackalloc byte[2];

            toBeCast[0] = Read();
            toBeCast[1] = Read();
            
            if (!BitConverter.IsLittleEndian)
                toBeCast.Reverse();
            
            return MemoryMarshal.Cast<byte, ushort>(toBeCast)[0];
        }
    }
}