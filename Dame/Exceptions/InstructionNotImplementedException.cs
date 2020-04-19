using System;

namespace Dame.Exceptions
{
    class InstructionNotImplementedException : Exception
    {
        public int Opcode { get; }

        public InstructionNotImplementedException(int opcode)
            : this(opcode, "This instruction is not implemented!", null)
        { }

        public InstructionNotImplementedException(int opcode, string message)
            : this(opcode, message, null)
        { }

        public InstructionNotImplementedException(int opcode, string message, Exception innerException)
            : base($"{message}\n Opcode: 0x{opcode.ToString("X")}", innerException)
        {
            Opcode = opcode;
        }
    }
}