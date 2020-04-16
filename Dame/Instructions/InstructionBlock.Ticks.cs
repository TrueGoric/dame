using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dame.Accessors;
using Dame.Architecture;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBlock
    {
        private static readonly MethodInfo stepMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.Step));

        public InstructionBlock Cycle(int cycles = 1)
        {
            for (int i = 0; i < cycles; i++)
                expressions.Add((ExpressionGroup.Synchronization, Expression.Call(Expression.Constant(context), stepMethod)));

            return this;
        }
    }
}