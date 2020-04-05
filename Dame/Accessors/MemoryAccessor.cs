using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dame.Memory;

namespace Dame.Accessors
{
    sealed class MemoryAccessor<T>
        where T : unmanaged
    {
        private readonly MemoryController<T> memoryController;

        public MemoryAccessor(MemoryController<T> controller)
        {
            memoryController = controller;
        }

        public int Location { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read() => memoryController.Read(Location++);

        public unsafe TMarshal ReadCast<TMarshal>()
            where TMarshal : unmanaged
        {
            Span<T> toBeCast = stackalloc T[sizeof(TMarshal)];

            for (int i = 0; i < sizeof(TMarshal); i++)
                toBeCast[i] = Read();
            
            return MemoryMarshal.Cast<T, TMarshal>(toBeCast)[0];
        }
    }
}