using System;
using System.Runtime.InteropServices;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Processor
{
    public sealed class RegisterBank : IEmulationState
    {
        private byte[] registers;

        #region Register Accessors

        // nice names for special registers
        public byte Accumulator
        {
            get => registers[7];
            set => registers[7] = value;
        }

        public byte Flags
        {
            get => registers[6];
            set => registers[6] = value;
        }

        public byte B
        {
            get => registers[1];
            set => registers[1] = value; // TODO: figure out how to handle differing endianness between archs, especially when saving snapshots
        }

        public byte C
        {
            get => registers[0];
            set => registers[0] = value;
        }

        public byte D

        {
            get => registers[3];
            set => registers[3] = value;
        }

        public byte E
        {
            get => registers[2];
            set => registers[2] = value;
        }

        public byte H
        {
            get => registers[5];
            set => registers[5] = value;
        }

        public byte L
        {
            get => registers[4];
            set => registers[4] = value;
        }

        public byte A
        {
            get => registers[7];
            set => registers[7] = value;
        }

        public byte F
        {
            get => registers[6];
            set => registers[6] = value;
        }


        // 16-bit registers
        public ushort BC
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 0, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 0, 2))[0] = value;
        }

        public ushort DE
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 2, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 2, 2))[0] = value;
        }

        public ushort HL
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 4, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 4, 2))[0] = value;
        }

        public ushort SP
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 8, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 8, 2))[0] = value;
        }

        public ushort PC
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 10, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 10, 2))[0] = value;
        }

        public ushort AF
        {
            get => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 6, 2))[0];
            set => MemoryMarshal.Cast<byte, ushort>(new Span<byte>(registers, 6, 2))[0] = value;
        }

        #endregion

        #region Setters

        public void SetFlags(byte value, ProcessorFlags mask)
            => Flags = (byte)((value & (byte)mask) | (Flags & ~(byte)mask));
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

        public RegisterBank()
        {
            registers = new byte[12];
        }

        public ReadOnlySpan<byte> CreateSnapshot()
        {
            return registers;
        }

        public void RestoreSnapshot(ReadOnlySpan<byte> snapshot)
        {
            snapshot.CopyTo(registers);
        }
    }
}