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
        public InstructionBlock TestBit<T>(ParameterExpression variable, int bit)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            var mask = 1 << bit;

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, true)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(
                    Expression.And(variable, Expression.Constant(mask)),
                    Expression.Constant(0)
                )
            ))); // variable & mask == 0

            return this;
        }

        public InstructionBlock SetBit<T>(ParameterExpression variable, int bit, bool value)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            if (value)
            {
                var orMask = 1 << bit;
                expressions.Add((ExpressionGroup.Bit, Expression.OrAssign(variable, Expression.Constant(orMask))));
            }
            else
            {
                var andMask = ~(1 << bit);
                expressions.Add((ExpressionGroup.Bit, Expression.AndAssign(variable, Expression.Constant(andMask))));
            }

            return this;
        }
    }
}