using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Emulator.Accessors;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Instructions
{
    public sealed partial class InstructionBlock
    {
        public InstructionBlock TestBit<T>(ParameterExpression variable, int bit)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            var mask = 1 << bit;

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, true)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(
                    Expression.And(variable, Expression.Convert(Expression.Constant(mask), typeof(T))),
                    Expression.Constant((byte)0)
                )
            ))); // variable & mask == 0

            return this;
        }

        public InstructionBlock SetBit<T>(ParameterExpression variable, int bit, bool value)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            if (value)
            {
                var orMask = 1 << bit;
                expressions.Add((ExpressionGroup.Bit, Expression.OrAssign(variable, Expression.Convert(Expression.Constant(orMask), typeof(T)))));
            }
            else
            {
                var andMask = ~(1 << bit);
                expressions.Add((ExpressionGroup.Bit, Expression.AndAssign(variable, Expression.Convert(Expression.Constant(andMask), typeof(T)))));
            }

            return this;
        }
    }
}