using System.Runtime.InteropServices;
using Dame.Processor;

namespace Dame.Accessors
{
    sealed class RegisterAccessor
    {
        private readonly EmulationState emulationState;

        public EmulationState State { get; }

        #region Register Accessors

        // nice names for special registers
        public byte Accumulator
        {
            get => emulationState.Registers[7];
            set => emulationState.Registers[7] = value;
        }

        public byte Flags
        {
            get => emulationState.Registers[6];
            set => emulationState.Registers[6] = value;
        }

        public byte B
        {
            get => emulationState.Registers[0];
            set => emulationState.Registers[0] = value;
        }

        public byte C
        {
            get => emulationState.Registers[1];
            set => emulationState.Registers[1] = value;
        }

        public byte D

        {
            get => emulationState.Registers[2];
            set => emulationState.Registers[2] = value;
        }

        public byte E
        {
            get => emulationState.Registers[3];
            set => emulationState.Registers[3] = value;
        }

        public byte H
        {
            get => emulationState.Registers[4];
            set => emulationState.Registers[4] = value;
        }

        public byte L
        {
            get => emulationState.Registers[5];
            set => emulationState.Registers[5] = value;
        }

        public byte A
        {
            get => emulationState.Registers[6];
            set => emulationState.Registers[6] = value;
        }

        public byte F
        {
            get => emulationState.Registers[7];
            set => emulationState.Registers[7] = value;
        }


        // 16-bit registers
        public ushort BC
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[0..1])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[0..1])[0] = value;
        }

        public ushort DE
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[2..3])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[2..3])[0] = value;
        }

        public ushort HL
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[4..5])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[4..5])[0] = value;
        }

        public ushort SP
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[8..9])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[8..9])[0] = value;
        }

        public ushort PC
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[10..11])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[10..11])[0] = value;
        }

        public ushort AF
        {
            get => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[6..7])[0];
            set => MemoryMarshal.Cast<byte, ushort>(emulationState.Registers[6..7])[0] = value;
        }

        #endregion

        #region Setters

        public void SetFlags(byte value, ProcessorFlags mask)
            => Flags = (byte)((value & (byte)mask) & (Flags & ~(byte)mask));
        public void SetFlags(byte value)
            => Flags = value;
            
        public void SetAccumulator(byte value)
            => Accumulator = value;

        public void SetA(byte value)
            => A = value;
        public void SetB(byte value)
            => B = value;
        public void SetC(byte value)
            => C = value;
        public void SetD(byte value)
            => D = value;
        public void SetE(byte value)
            => E = value;
        public void SetH(byte value)
            => H = value;
        public void SetL(byte value)
            => L = value;
        public void SetF(byte value)
            => F = value;
        
        public void SetBC(ushort value)
            => BC = value;
        public void SetDE(ushort value)
            => DE = value;
        public void SetHL(ushort value)
            => HL = value;
        public void SetSP(ushort value)
            => SP = value;
        public void SetPC(ushort value)
            => PC = value;
        public void SetAF(ushort value)
            => AF = value;

        #endregion

        public RegisterAccessor(EmulationState state)
        {
            State = state;
        }
    }
}