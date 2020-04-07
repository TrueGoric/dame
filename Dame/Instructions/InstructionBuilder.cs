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

        private ProcessorFlags flagsRead = ProcessorFlags.None;

        private int cyclesUsed;

        private ParameterExpression flagsVariable;

        private List<(ExpressionGroup Group, Expression Expr)> expressions;



        public InstructionBuilder(string name, int cycles)
        {
            cyclesUsed = cycles;

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