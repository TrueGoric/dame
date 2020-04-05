using System.Runtime.InteropServices;

namespace Dame.Accessors
{
    sealed class RegisterAccessor
    {
        private readonly EmulationState emulationState;

        public EmulationState State { get; }

        public byte[] Raw => State.Registers;

        #region Register Accessors

        // nice names for special registers
        public ref byte Accumulator
            => ref emulationState.Registers[7];
        public ref byte Flags
            => ref emulationState.Registers[6];

        
        public ref byte B
            => ref emulationState.Registers[0];
        public ref byte C
            => ref emulationState.Registers[1];
        public ref byte D
            => ref emulationState.Registers[2];
        public ref byte E
            => ref emulationState.Registers[3];
        public ref byte H
            => ref emulationState.Registers[4];
        public ref byte L
            => ref emulationState.Registers[5];
        public ref byte F
            => ref emulationState.Registers[6];
        public ref byte A
            => ref emulationState.Registers[7];

        // 16-bit registers
        public ref ushort BC
            => ref MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[0..1])[0];
        public ref ushort DE
            => ref MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[2..3])[0];
        public ref ushort HL
            => ref MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[4..5])[0];
        public ref ushort SP
            => ref MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[8..9])[0];
        public ref ushort PC
            => ref MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[10..11])[0];

        #endregion

        public RegisterAccessor(EmulationState state)
        {
            State = state;
        }
    }
}