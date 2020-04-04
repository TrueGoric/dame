using System;
using System.Runtime.InteropServices;

namespace Dame
{
    class EmulationState : IDisposable
    {
        private readonly byte[] registers;

        #region Register Accessors

        // nice names for special registers
        public ref byte Accumulator
            => ref registers[0];
        public ref byte Flags
            => ref registers[1];

        public ref byte A
            => ref registers[0];
        public ref byte F
            => ref registers[1];
        public ref byte B
            => ref registers[2];
        public ref byte C
            => ref registers[3];
        public ref byte D
            => ref registers[4];
        public ref byte E
            => ref registers[5];
        public ref byte H
            => ref registers[6];
        public ref byte L
            => ref registers[7];

        // 16-bit registers
        public ref ushort BC
            => ref MemoryMarshal.Cast<byte, ushort>(registers[2..3])[0];
        public ref ushort DE
            => ref MemoryMarshal.Cast<byte, ushort>(registers[4..5])[0];
        public ref ushort HL
            => ref MemoryMarshal.Cast<byte, ushort>(registers[6..7])[0];
        public ref ushort SP
            => ref MemoryMarshal.Cast<byte, ushort>(registers[8..9])[0];
        public ref ushort PC
            => ref MemoryMarshal.Cast<byte, ushort>(registers[10..11])[0];

        #endregion

        public EmulationState()
        {
            registers = new byte[12];
        }

        public EmulationState CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}