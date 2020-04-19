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
        public InstructionBlock Input<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, int fetchCycles = 0)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            expressions.Add((ExpressionGroup.IO, Expression.Assign(variable, expression.Body)));
            
            if (fetchCycles > 0)
                Cycle(fetchCycles);

            return this;
        }

        public InstructionBlock Output<T>(ParameterExpression variable, Expression<InstructionFunction<T>> expression, int writeCycles = 0)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            expressions.Add((ExpressionGroup.IO, Expression.Invoke(expression, new[] { variable })));

            if (writeCycles > 0)
                Cycle(writeCycles);

            return this;
        }
    }
}