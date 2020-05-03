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
        public InstructionBlock Add<T, U>(ParameterExpression variable, Expression<InstructionValue<U>> expression, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
            where U : unmanaged
            => Add<T, U>(variable, expression.Body, withCarry);

        public InstructionBlock Add<T, U>(ParameterExpression variable, U value, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
            where U : unmanaged
            => Add<T, U>(variable, Expression.Constant(value, typeof(U)), withCarry);

        public InstructionBlock Add<T, U>(ParameterExpression variable, Expression expression, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
            where U : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnUnsupportedType<U>();
            ThrowOnExpressionTypeMismatch<T>(variable);
            ThrowOnExpressionTypeMismatch<U>(expression);

            variables.Add(variable);

            var carryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>()
                : GetMaxValue<U>();
            var halfCarryMask = GetMaxValue<T>() < GetMaxValue<U>()
                ? GetMaxValue<T>() >> 4
                : GetMaxValue<U>() >> 4;

            var expressionResult = Expression.Variable(typeof(U), "expressionResult");
            variables.Add(expressionResult);

            var carryValue = GetCarryFlagValue();

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            if (withCarry)
                expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, WrappedAdd<U>(expressionResult, carryValue))));

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry,
                    TestAddCarry(carryMask, variable, expressionResult))));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry,
                    TestAddCarry(halfCarryMask, variable, expressionResult)
                    )));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(variable, WrappedAdd<T>(variable, expressionResult)))); // unchecked wrapped add

            if (setFlags)
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                    Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Subtract<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
            => Subtract<T>(variable, expression.Body, withCarry, setFlags);

        public InstructionBlock Subtract<T>(ParameterExpression variable, T value, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
            => Subtract<T>(variable, Expression.Constant(value, typeof(T)), withCarry, setFlags);

        public InstructionBlock Subtract<T>(ParameterExpression variable, Expression expression, bool withCarry = false, bool setFlags = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            var carryMask = GetMaxValue<T>();
            var halfCarryMask = GetMaxValue<T>() >> 4;

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            var carryValue = GetCarryFlagValue();
            var expressionAndCarry = WrappedAdd<T>(expressionResult, carryValue);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, expression)));

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry,
                    TestSubCarry<T>(carryMask, variable, expressionResult, withCarry ? carryValue : null))));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry,
                    TestSubCarry<T>(halfCarryMask, variable, expressionResult, withCarry ? carryValue : null)
                    )));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(variable, WrappedSub<T>(
                variable,
                withCarry ? (Expression)expressionAndCarry : expressionResult
            ))));

            if (setFlags)
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                    Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Or<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, bool setFlags = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, Expression.Invoke(expression))));

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.OrAssign(variable, expressionResult)));

            if (setFlags)
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                    Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock And<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, bool setFlags = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, Expression.Invoke(expression))));

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.AndAssign(variable, expressionResult)));

            if (setFlags)
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                    Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Xor<T>(ParameterExpression variable, Expression<InstructionValue<T>> expression, bool setFlags = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            var expressionResult = Expression.Variable(typeof(T), "expressionResult");
            variables.Add(expressionResult);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.Assign(expressionResult, Expression.Invoke(expression))));

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.ExclusiveOrAssign(variable, expressionResult)));

            if (setFlags)
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                    Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public InstructionBlock Complement<T>(ParameterExpression variable, bool setFlags = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<T>(variable);

            variables.Add(variable);

            if (setFlags)
            {
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, true)));
                expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, true)));
            }

            expressions.Add((ExpressionGroup.Arithmetic, Expression.ExclusiveOrAssign(variable, Expression.Convert(Expression.Constant(GetMaxValue<T>()), typeof(T)))));

            return this;
        }

        // https://forums.nesdev.com/viewtopic.php?t=15944#p196282
        public InstructionBlock DecimalAdjust<T>(ParameterExpression variable)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnExpressionTypeMismatch<byte>(variable); // currently supporting only bytes

            variables.Add(variable);

            var carryFlipVariable = Expression.Variable(typeof(bool), "carryFlip");
            variables.Add(carryFlipVariable);

            expressions.Add((ExpressionGroup.Arithmetic, Expression.IfThenElse(
                ReadFlag(ProcessorFlags.Arithmetic),
                Expression.Block(
                    Expression.IfThen(
                        ReadFlag(ProcessorFlags.Carry),
                        Expression.Assign(variable, WrappedSub<T>(variable, Expression.Constant((byte)0x60)))
                    ),
                    Expression.IfThen(
                        ReadFlag(ProcessorFlags.HalfCarry),
                        Expression.Assign(variable, WrappedSub<T>(variable, Expression.Constant((byte)0x6)))
                    )
                ),
                Expression.Block(
                    Expression.IfThen(
                        Expression.OrElse(ReadFlag(ProcessorFlags.Carry), Expression.GreaterThan(Expression.Convert(variable, typeof(long)), Expression.Constant(0x99L))),
                        Expression.Block(
                            Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(0x60))),
                            Expression.Assign(carryFlipVariable, Expression.Constant(true))
                        )
                    ),
                    Expression.IfThen(
                        Expression.OrElse(
                            ReadFlag(ProcessorFlags.HalfCarry),
                            Expression.GreaterThan(
                                Expression.Convert(Expression.And(variable, Expression.Convert(Expression.Constant(0x0F), typeof(T))), typeof(long)),
                                Expression.Constant(0x09L)
                            )
                        ),
                        Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(0x6)))
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
                case TypeCode.SByte:
                    return sbyte.MaxValue;
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
                        Expression.Convert(Expression.And(firstExpression, Expression.Convert(Expression.Constant(mask), firstExpression.Type)), typeof(long)),
                        Expression.Convert(Expression.And(secondExpression, Expression.Convert(Expression.Constant(mask), secondExpression.Type)), typeof(long))
                    ),
                    Expression.Constant(mask)
                ); // (first & mask) + (second & mask) > mask
        
        private Expression TestSubCarry<T>(long mask, Expression firstExpression, Expression secondExpression, Expression carryExpression = null)
            where T : unmanaged
        {
            if (mask == GetMaxValue<T>())
                return Expression.LessThan(
                    firstExpression,
                    carryExpression == null
                        ? secondExpression
                        : WrappedAdd<T>(secondExpression, carryExpression)
                );
            else
                return Expression.NotEqual(
                    Expression.And(
                        Expression.Convert(
                            carryExpression == null
                            ? WrappedSub<T>(
                                Expression.And(firstExpression, Expression.Convert(Expression.Constant(mask), typeof(T))),
                                Expression.And(secondExpression, Expression.Convert(Expression.Constant(mask), typeof(T)))
                            )
                            : WrappedSub<T>(
                                WrappedSub<T>(
                                    Expression.And(firstExpression, Expression.Convert(Expression.Constant(mask), typeof(T))),
                                    Expression.And(secondExpression, Expression.Convert(Expression.Constant(mask), typeof(T)))
                                ),
                                carryExpression
                            ), typeof(long)
                        ),
                        Expression.Constant(mask + 1)
                    ),
                    Expression.Constant(0L)
                ); // (first & mask - second & mask [- carry]) & (mask + 1) != 0
        }
        
        private Expression WrappedAdd<T>(Expression leftExpression, Expression rightExpression)
            where T : unmanaged
            => Expression.Convert(
                Expression.Add(
                    Expression.Convert(leftExpression, typeof(long)),
                    Expression.Convert(rightExpression, typeof(long))
                ),
                typeof(T)
            );

        private unsafe Expression WrappedSub<T>(Expression leftExpression, Expression rightExpression)
            where T : unmanaged
            => Expression.Convert(
                Expression.Subtract(
                    Expression.Convert(leftExpression, typeof(long)),
                    Expression.Convert(rightExpression, typeof(long))
                ),
                typeof(T)
            );
    }
}