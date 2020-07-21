using System;
using Dame.Emulator.Processor;

namespace Dame.Emulator.JIT
{
    public delegate void JITBlockMethod(ProcessorExecutionContext context, int instructionOffset);

    public readonly struct JITBlock
    {
        public int InstructionLength { get; }
        public JITBlockMethod Invoker { get; }

        public JITBlock(JITBlockMethod invoker, int instructions)
        {
            Invoker = invoker;
            InstructionLength = instructions;
        }
    }
}
