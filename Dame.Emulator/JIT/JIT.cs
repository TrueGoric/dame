using System;
using Dame.Emulator.Instructions;
using Dame.Emulator.Processor;

namespace Dame.Emulator.JIT
{
    public sealed partial class JIT
    {
        public bool PrintDisassemblyOnExecution { get; set; }
        public int InstructionsPerBlock { get; set; }

        public Instruction Compile(ushort opcode)
        {
            throw new NotImplementedException();
        }

        public JITBlock Compile(ReadOnlySpan<byte> assembly)
        {
            throw new NotImplementedException();
        }
    }
}
