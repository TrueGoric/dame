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
        public unsafe InstructionBlock Swap<T>(ParameterExpression variable)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);

            var halfSize = sizeof(T) / 2;

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Shift, Expression.Assign(variable, 
                Expression.Or(
                    Expression.LeftShift(variable, Expression.Constant(halfSize)),
                    Expression.RightShift(variable, Expression.Constant(halfSize))
                )
            )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public unsafe InstructionBlock RotateLeft<T>(ParameterExpression variable, bool overrideCarry = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);
            
            var size = sizeof(T) * 4;
            var lowMask = 1;
            var highMask = 1 << (size - 1);

            var carryVariable = Expression.Variable(typeof(bool), "carryHold");
            variables.Add(carryVariable);

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Shift, Expression.Assign(
                carryVariable,
                Expression.GreaterThan(
                    Expression.And(variable, Expression.Convert(Expression.Constant(highMask), typeof(T))),
                    Expression.Constant((byte)0)
                )
            )));

            expressions.Add((ExpressionGroup.Shift, Expression.LeftShiftAssign(variable, Expression.Constant(1))));
            
            if (overrideCarry)
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    carryVariable,
                    Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(lowMask)))
                )));
            else
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    ReadFlag(ProcessorFlags.Carry),
                    Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(lowMask)))
                )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, carryVariable)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public unsafe InstructionBlock RotateRight<T>(ParameterExpression variable, bool overrideCarry = true)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);
            
            var size = sizeof(T) * 4;
            var lowMask = 1;
            var highMask = 1 << (size - 1);

            var carryVariable = Expression.Variable(typeof(bool), "carryHold");
            variables.Add(carryVariable);

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Shift, Expression.Assign(
                carryVariable,
                Expression.GreaterThan(
                    Expression.And(variable, Expression.Convert(Expression.Constant(lowMask), typeof(T))),
                    Expression.Constant((byte)0)
                )
            )));
            
            expressions.Add((ExpressionGroup.Shift, Expression.RightShiftAssign(variable, Expression.Constant(1))));
            
            if (overrideCarry)
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    carryVariable,
                    Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(highMask)))
                )));
            else
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    ReadFlag(ProcessorFlags.Carry),
                    Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(highMask)))
                )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, carryVariable)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public unsafe InstructionBlock ShiftLeft<T>(ParameterExpression variable, bool copyBit = false)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);
            
            var size = sizeof(T) * 4;
            var lowMask = 1;
            var highMask = 1 << (size - 1);
            var nearLowMask = lowMask << 1;

            var carryVariable = Expression.Variable(typeof(bool), "carryHold");
            variables.Add(carryVariable);

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Shift, Expression.Assign(
                carryVariable,
                Expression.GreaterThan(
                    Expression.And(variable, Expression.Convert(Expression.Constant(highMask), typeof(T))),
                    Expression.Constant((byte)0)
                )
            )));

            expressions.Add((ExpressionGroup.Shift, Expression.LeftShiftAssign(variable, Expression.Constant(1))));
            
            if (copyBit)
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    Expression.GreaterThan(Expression.And(variable, Expression.Constant(nearLowMask)), Expression.Constant(0)),
                    Expression.AddAssign(variable, Expression.Constant(lowMask))
                )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, carryVariable)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }

        public unsafe InstructionBlock ShiftRight<T>(ParameterExpression variable, bool copyBit = false)
            where T : unmanaged
        {
            ThrowOnUnsupportedType<T>();
            ThrowOnVariableTypeMismatch<T>(variable);

            variables.Add(variable);
            
            var size = sizeof(T) * 4;
            var lowMask = 1;
            var highMask = 1 << (size - 1);
            var nearHighMask = highMask >> 1;

            var carryVariable = Expression.Variable(typeof(bool), "carryHold");
            variables.Add(carryVariable);

            // set flags
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Arithmetic, false)));
            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.HalfCarry, false)));

            expressions.Add((ExpressionGroup.Shift, Expression.Assign(
                carryVariable,
                Expression.GreaterThan(
                    Expression.And(variable, Expression.Convert(Expression.Constant(lowMask), typeof(T))),
                    Expression.Constant((byte)0)
                )
            )));

            expressions.Add((ExpressionGroup.Shift, Expression.RightShiftAssign(variable, Expression.Constant(1))));
            
            if (copyBit)
                expressions.Add((ExpressionGroup.Shift, Expression.IfThen(
                    Expression.GreaterThan(Expression.And(variable, Expression.Convert(Expression.Constant(nearHighMask), typeof(T))), Expression.Constant((byte)0)),
                    Expression.Assign(variable, WrappedAdd<T>(variable, Expression.Constant(highMask)))
                )));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Carry, carryVariable)));

            expressions.Add((ExpressionGroup.Flags, CreateFlagAssignExpression(ProcessorFlags.Zero,
                Expression.Equal(variable, Expression.Default(typeof(T)))))); // variable == default(T)

            return this;
        }
    }
}