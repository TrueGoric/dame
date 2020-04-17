using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBlock
    {
        public InstructionBlock IfFlagsSet(ProcessorFlags flags, Action<InstructionBlock> blockSet)
        {
            var ifTrueBlock = new InstructionBlock(context, instructionContext);

            blockSet(ifTrueBlock);

            instructionContext.FlagsRead |= flags;

            expressions.Add((ExpressionGroup.Conditional, new InstructionCondition(
                Expression.GreaterThan(
                    Expression.And(
                        Expression.Convert(instructionContext.FlagsVariable, typeof(byte)),
                        Expression.Constant((byte)flags, typeof(byte))
                    ),
                    Expression.Constant(0, typeof(byte))
                ),
                ifTrueBlock
            )));

            return this;
        }

        private static ConditionalExpression GenerateConditionalExpression(InstructionCondition condition)
        {
            if (condition.IfFalse != null)
                return Expression.IfThenElse(condition.Condition,
                    Expression.Block(PopulateExpressions(condition.IfTrue).Select(e => e.Expr)),
                    Expression.Block(PopulateExpressions(condition.IfFalse).Select(e => e.Expr)));
            else
                return Expression.IfThen(condition.Condition,
                    Expression.Block(PopulateExpressions(condition.IfTrue).Select(e => e.Expr)));
        }
    }
}