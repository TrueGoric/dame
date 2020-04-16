using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Architecture;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBlock
    {
        private readonly ProcessorExecutionContext context;
        
        private List<(ExpressionGroup Group, Expression Expr)> expressions;
        private ParameterExpression flagsVariable;

        private ProcessorFlags flagsRead = ProcessorFlags.None;

        public List<(ExpressionGroup Group, Expression Expr)> Expressions => expressions;
        public ProcessorFlags FlagsRead => flagsRead;

        public InstructionBlock(ProcessorExecutionContext context)
        {
            this.context = context;
            
            expressions = new List<(ExpressionGroup Group, Expression Expr)>();
            flagsVariable = Expression.Variable(typeof(byte));
        }

        private void ThrowOnUnsupportedType<T>()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
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