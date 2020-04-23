using System;

namespace Dame.Emulator.Exceptions
{
    public class InstructionNotImplementedException : Exception
    {
        public int Opcode { get; }

        public InstructionNotImplementedException(int opcode)
            : this(opcode, "This instruction is not implemented!", null)
        { }

        public InstructionNotImplementedException(int opcode, string message)
            : this(opcode, message, null)
        { }

        public InstructionNotImplementedException(int opcode, string message, Exception innerException)
            : base($"Opcode: 0x{opcode.ToString("X")} :: {message}", innerException)
        {
            Opcode = opcode;
        }
    }
}