using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBuilder
    {
        private enum ExpressionGroup
        {
            IO,
            Flags,
            Arithmetic,
            Conditional
        }

        private int opcode;
        private string mnemonic;
        private int cycles;

        private ParameterExpression flagsVariable;
        private ProcessorFlags flagsRead = ProcessorFlags.None;

        private List<(ExpressionGroup Group, Expression Expr)> expressions;

        public InstructionBuilder(int opcode, string mnemonic, int cycles)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.cycles = cycles;

            flagsVariable = Expression.Variable(typeof(byte));
        }

        public Instruction Compile()
        {
            throw new NotImplementedException();
        }

        private void ThrowOnUnsupportedType<T>()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    return;

                default:
                throw new NotSupportedException($"Type {typeof(T).FullName} is not supported by {nameof(InstructionBuilder)}!");
            }
        }

        private void ThrowOnVariableTypeMismatch<T>(ParameterExpression variable)
        {
            if (variable.Type != typeof(T))
                throw new ArgumentException("Provided expression is not of the declared generic type!", nameof(variable));
        }
    }
}