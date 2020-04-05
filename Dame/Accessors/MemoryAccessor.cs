using System;
using System.Runtime.CompilerServices;
using Dame.Memory;

namespace Dame.Accessors
{
    sealed class MemoryAccessor<T>
        where T : struct
    {
        private readonly MemoryController<T> memoryController;

        public MemoryAccessor(MemoryController<T> controller)
        {
            memoryController = controller;
        }

        public int Location { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read() => memoryController.Read(Location++);
    }
}