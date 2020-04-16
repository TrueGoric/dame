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
        public InstructionBlock IfFlagsSet(ProcessorFlags flags, Expression<InstructionAction> expression)
            => IfFlagsSetInternal(flags, Expression.Invoke(expression));
        
        public InstructionBlock IfFlagsSet<T>(ProcessorFlags flags, ParameterExpression variable, Expression<InstructionFunction<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return IfFlagsSetInternal(flags, Expression.Invoke(expression, new[] { variable }));
        }

        public InstructionBlock IfFlagsSetInternal(ProcessorFlags flags, Expression expression)
        {
            flagsRead |= flags;

            expressions.Add((ExpressionGroup.Conditional, Expression.IfThen(
                Expression.GreaterThan(
                    Expression.And(
                        Expression.Convert(flagsVariable, typeof(byte)),
                        Expression.Constant((byte)flags, typeof(byte))
                    ),
                    Expression.Constant(0, typeof(byte))
                ),
                expression
            )));

            return this;
        }
    }
}