using System;

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

        // spans for 16-bit registers
        public Span<byte> BC
            => registers[2..3];
        public Span<byte> DE
            => registers[4..5];
        public Span<byte> HL
            => registers[6..7];
        public Span<byte> SP
            => registers[8..9];
        public Span<byte> PC
            => registers[10..11];

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