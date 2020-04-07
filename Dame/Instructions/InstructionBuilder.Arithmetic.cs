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
        public InstructionBuilder Add<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            var carryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>()
                : GetMaxValue<U>();
            var halfCarryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>() >> 4
                : GetMaxValue<U>() >> 4;

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry,
                CreateTestAddCarryExpression(halfCarryMask, variable, expressionResult))));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry,
                CreateTestAddCarryExpression(halfCarryMask, variable, expressionResult)
                )));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.AddAssign(variable, expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBuilder Subtract<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return this;
        }

        public InstructionBuilder Or<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return this;
        }

        public InstructionBuilder And<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return this;
        }

        public InstructionBuilder Xor<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return this;
        }

        public InstructionBuilder Complement<T>(ParameterExpression variable)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            return this;
        }

        private long GetMaxValue<T>()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                    return byte.MaxValue;
                case TypeCode.UInt16:
                    return ushort.MaxValue;
                case TypeCode.UInt32:
                    return uint.MaxValue;

                default:
                    throw new NotSupportedException($"Type {typeof(T).FullName} is not supported by {nameof(InstructionBuilder)}!");
            }
        }

        private Expression CreateTestAddCarryExpression(long mask, Expression firstExpression, Expression secondExpression)
            => Expression.GreaterThan(
                    Expression.Add(
                        Expression.And(firstExpression, Expression.Constant(mask, typeof(long))),
                        Expression.And(secondExpression, Expression.Constant(mask, typeof(long)))
                    ),
                    Expression.Constant(mask, typeof(long))
                ); // (variable & mask) + (expressionResult & mask) > mask
    }
}