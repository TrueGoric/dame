using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBlock
    {
        public InstructionBlock WriteFlags(Expression<InstructionValue<byte>> expression)
        {
            expressions.Add((ExpressionGroup.IO, Expression.Assign(instructionContext.FlagsVariable, expression)));

            return this;
        }

        public InstructionBlock ReadFlags(Expression<InstructionFunction<byte>> expression, ProcessorFlags mask = ProcessorFlags.All)
        {
            instructionContext.FlagsRead |= mask;

            expressions.Add((ExpressionGroup.Flags, Expression.Invoke(expression, new[]
            {
                mask == ProcessorFlags.All
                ? (Expression) instructionContext.FlagsVariable
                : (Expression) Expression.And(instructionContext.FlagsVariable, Expression.Constant(mask, typeof(byte)))
            })));

            return this;
        }

        public InstructionBlock SetFlags(ProcessorFlags flags)
        {
            expressions.Add((ExpressionGroup.Flags,
                Expression.OrAssign(
                    instructionContext.FlagsVariable,
                    Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte))
                )));

            return this;
        }

        public InstructionBlock UnsetFlags(ProcessorFlags flags)
        {
            expressions.Add((ExpressionGroup.Flags,
                Expression.AndAssign(
                    instructionContext.FlagsVariable,
                    Expression.OnesComplement(Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte)))
                )));
                
            return this;
        }

        private Expression CreateFlagAssignExpression(ProcessorFlags flags, Expression condition)
            => Expression.IfThenElse(condition,
                Expression.OrAssign(instructionContext.FlagsVariable, Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte))),
                Expression.AndAssign(instructionContext.FlagsVariable, Expression.OnesComplement(Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte)))));
        
        private Expression CreateFlagAssignExpression(ProcessorFlags flags, bool assignment)
            => assignment
                ? Expression.OrAssign(instructionContext.FlagsVariable, Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte)))
                : Expression.AndAssign(instructionContext.FlagsVariable, Expression.OnesComplement(Expression.Convert(Expression.Constant(flags, typeof(ProcessorFlags)), typeof(byte))));
        
        private Expression GetCarryFlagValue()
            => Expression.And(Expression.RightShift(instructionContext.FlagsVariable, Expression.Constant(4)), Expression.Constant(0x1));
        
        private Expression ReadFlag(ProcessorFlags flag)
            => Expression.GreaterThan(
                Expression.And(instructionContext.FlagsVariable, Expression.Constant(flag)),
                Expression.Constant(1)
            );
    }
}