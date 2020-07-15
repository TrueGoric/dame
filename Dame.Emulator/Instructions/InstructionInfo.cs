using System;

namespace Dame.Emulator.Instructions
{
    public readonly struct InstructionInfo
    {
        public InstructionType Type { get; }

        public Operand FirstArgument { get; }
        public Operand SecondArgument { get; }

        public InstructionInfo(InstructionType type, Operand firstArgument, Operand secondArgument)
        {
            Type = type;
            FirstArgument = firstArgument;
            SecondArgument = secondArgument;
        }
    }
}
