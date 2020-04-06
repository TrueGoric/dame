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
            if (sizeof(TMarshal) % sizeof(T) != 0)
                throw new NotSupportedException($"Base ({typeof(T)}) and marshaled ({typeof(TMarshal)}) types must be divisible in length!");
            
            var castSize = sizeof(TMarshal) / sizeof(T);
            Span<T> toBeCast = stackalloc T[castSize];

            for (int i = 0; i < castSize; i++)
                toBeCast[i] = Read();
            
            if (!BitConverter.IsLittleEndian)
                toBeCast.Reverse();
            
            return MemoryMarshal.Cast<T, TMarshal>(toBeCast)[0];
        }
    }
}