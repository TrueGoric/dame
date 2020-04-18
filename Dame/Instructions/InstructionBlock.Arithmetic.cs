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
        public InstructionBlock Add<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression, bool withCarry = false)
            where T : unmanaged
            where U : unmanaged
            => Add<T, U>(variable, expression);

        public InstructionBlock Add<T, U>(ParameterExpression variable, U value, bool withCarry = false)
            where T : unmanaged
            where U : unmanaged
            => Add<T, U>(variable, Expression.Constant(value, typeof(U)));

        private InstructionBlock Add<T, U>(ParameterExpression variable, Expression expression, bool withCarry = false)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var carryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>()
                : GetMaxValue<U>();
            var halfCarryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>() >> 4
                : GetMaxValue<U>() >> 4;

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            var carryValue = GetCarryFlagValue();

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            if (withCarry)
                expressions.Add((ExpressionGroup.Arithmetic, Expression.AddAssign(expressionResult, carryValue)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry,
                TestAddCarry(carryMask, variable, expressionResult))));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry,
                TestAddCarry(halfCarryMask, variable, expressionResult)
                )));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.AddAssign(variable, expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Subtract<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, bool withCarry = false)
            where T : unmanaged
            => Subtract<T>(variable, expression);

        public InstructionBlock Subtract<T>(ParameterExpression variable, T value, bool withCarry = false)
            where T : unmanaged
            => Subtract<T>(variable, Expression.Constant(value, typeof(T)));

        private InstructionBlock Subtract<T>(ParameterExpression variable, Expression expression, bool withCarry = false)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var carryMask = GetMaxValue<T>();
            var halfCarryMask = GetMaxValue<T>() >> 4;

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            var carryValue = GetCarryFlagValue();
            var expressionAndCarry = Expression.Add(expressionResult, carryValue);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry,
                TestSubCarry<T>(carryMask, variable, expressionResult, withCarry ? carryValue : null))));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry,
                TestSubCarry<T>(halfCarryMask, variable, expressionResult, withCarry ? carryValue : null)
                )));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.SubtractAssign(variable, withCarry ? (Expression)expressionAndCarry : expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Or<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.OrAssign(variable, expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock And<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.AndAssign(variable, expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Xor<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.ExclusiveOrAssign(variable, expressionResult)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Complement<T>(ParameterExpression variable)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, true)));

            expressions.Add((ExpressionGroup.Arithmetic, Expression.ExclusiveOrAssign(variable, Expression.Constant(GetMaxValue<T>(), typeof(T)))));

            return this;
        }

        // https://forums.nesdev.com/viewtopic.php?t=15944#p196282
        public InstructionBlock DecimalAdjust<T>(ParameterExpression variable)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<byte>(variable); // currently supporting only bytes

            variables.Add(variable);

            var carryFlipVariable = Expression.Variable(typeof(bool), "carryFlip");
            variables.Add(carryFlipVariable);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.IfThenElse(
                ReadFlag(ProcessorFlags.Arithmetic),
                Expression.Block(
                    Expression.IfThen(
                        ReadFlag(ProcessorFlags.Carry),
                        Expression.SubtractAssign(variable, Expression.Constant(0x60))
                    ),
                    Expression.IfThen(
                        ReadFlag(ProcessorFlags.HalfCarry),
                        Expression.SubtractAssign(variable, Expression.Constant(0x6))
                    )
                ),
                Expression.Block(
                    Expression.IfThen(
                        Expression.Or(ReadFlag(ProcessorFlags.Carry), Expression.GreaterThan(variable, Expression.Constant(0x99))),
                        Expression.Block(
                            Expression.AddAssign(variable, Expression.Constant(0x60)),
                            Expression.Assign(carryFlipVariable, Expression.Constant(true))
                        )
                    ),
                    Expression.IfThen(
                        Expression.Or(ReadFlag(ProcessorFlags.HalfCarry), Expression.GreaterThan(Expression.And(variable, Expression.Constant(0x0F)), Expression.Constant(0x09))),
                        Expression.AddAssign(variable, Expression.Constant(0x6))
                    )
                )
            )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, carryFlipVariable)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

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

        private Expression TestAddCarry(long mask, Expression firstExpression, Expression secondExpression)
            => Expression.GreaterThan(
                    Expression.Add(
                        Expression.And(firstExpression, Expression.Constant(mask, typeof(long))),
                        Expression.And(secondExpression, Expression.Constant(mask, typeof(long)))
                    ),
                    Expression.Constant(mask, typeof(long))
                ); // (variable & mask) + (expressionResult & mask) > mask
        
        private Expression TestSubCarry<T>(long mask, Expression firstExpression, Expression secondExpression, Expression carryExpression = null)
            where T : unmanaged
            => Expression.NotEqual(
                Expression.And(
                    carryExpression == null
                    ? WrappedSub<T>(
                        Expression.And(firstExpression, Expression.Constant(mask)),
                        Expression.And(secondExpression, Expression.Constant(mask))
                    )
                    : WrappedSub<T>(
                        WrappedSub<T>(
                            Expression.And(firstExpression, Expression.Constant(mask)),
                            Expression.And(secondExpression, Expression.Constant(mask))
                        ),
                        carryExpression
                    ),
                    Expression.Constant(mask + 1)
                ),
                Expression.Constant(0)
            );

        private unsafe Expression WrappedSub<T>(Expression leftExpression, Expression rightExpression)
            where T : unmanaged
            => Modulus(Expression.Subtract(leftExpression, rightExpression), Expression.Constant(sizeof(T)));

        private Expression Modulus(Expression leftExpression, Expression rightExpression)
            => Expression.Modulo(
                Expression.Add(
                    Expression.Modulo(leftExpression, rightExpression),
                    rightExpression
                ),
                rightExpression
            );
    }
}