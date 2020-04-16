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
        public InstructionBlock Input<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            expressions.Add((ExpressionGroup.IO, Expression.Assign(variable, Expression.Invoke(expression))));

            return this;
        }

        public InstructionBlock Output<T>(ParameterExpression variable, Expression<InstructionFunction<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            expressions.Add((ExpressionGroup.IO, Expression.Invoke(expression, new[] { variable })));

            return this;
        }
    }
}