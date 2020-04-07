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
        public InstructionBuilder WriteFlags(Expression<InstructionValue<byte>> expression)
        {
            expressions.Add((ExpressionGroup.IO, Expression.Assign(flagsVariable, expression)));

            return this;
        }

        public InstructionBuilder ReadFlags(Expression<InstructionFunction<byte>> expression, ProcessorFlags mask = ProcessorFlags.All)
        {
            flagsRead |= mask;

            expressions.Add((ExpressionGroup.Flags, Expression.Invoke(expression, new[]
            {
                mask == ProcessorFlags.All
                ? (Expression) flagsVariable
                : (Expression) Expression.And(flagsVariable, Expression.Constant(mask, typeof(byte)))
            })));

            return this;
        }

        public InstructionBuilder SetFlags(ProcessorFlags flag)
        {
            return this;
        }

        public InstructionBuilder UnsetFlags(ProcessorFlags flag)
        {
            return this;
        }

        private Expression CreateFlagAssignExpression(ProcessorFlags flags, Expression condition)
            => Expression.IfThenElse(condition,
                Expression.OrAssign(flagsVariable, Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte))),
                Expression.AndAssign(flagsVariable, Expression.OnesComplement(Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte)))));
        
        private Expression CreateFlagAssignExpression(ProcessorFlags flags, bool assignment)
            => assignment
                ? Expression.OrAssign(flagsVariable, Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte)))
                : Expression.AndAssign(flagsVariable, Expression.OnesComplement(Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte))));
    }
}