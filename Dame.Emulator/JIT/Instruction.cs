using System;
using Dame.Emulator.Processor;

namespace Dame.Emulator.JIT
{
    public delegate void InstructionMethod(ProcessorExecutionContext context);

    public readonly struct Instruction
    {
        public ushort OpCode { get; }
        public string Mnemonic { get; }
        public InstructionMethod Invoker { get; }

        public Instruction(ushort opcode, string mnemonic, InstructionMethod invoker)
        {
            OpCode = opcode;
            Mnemonic = mnemonic;
            Invoker = invoker;
        }
    }
}
