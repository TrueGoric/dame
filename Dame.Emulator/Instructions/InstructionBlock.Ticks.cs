using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dame.Emulator.Accessors;
using Dame.Emulator.Architecture;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Instructions
{
    public sealed partial class InstructionBlock
    {
        private static readonly MethodInfo stepMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.Cycle));

        public InstructionBlock Cycle(int cycles = 1)
        {
            expressions.Add((ExpressionGroup.Synchronization, Expression.Call(Expression.Constant(context), stepMethod, Expression.Constant(cycles))));

            return this;
        }
    }
}