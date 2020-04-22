using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dame.Emulator.Accessors;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Instructions
{
    public sealed partial class InstructionBlock
    {
        public InstructionBlock IfFlagsSet(ProcessorFlags flags, Action<InstructionBlock> ifTrueBlockSet, Action<InstructionBlock> ifFalseBlockSet = null)
        {
            InstructionBlock ifTrueBlock, ifFalseBlock = null;

            ifTrueBlock = new InstructionBlock(context, instructionContext);
            ifTrueBlockSet(ifTrueBlock);

            if (ifFalseBlockSet != null)
            {
                ifFalseBlock = new InstructionBlock(context, instructionContext);
                ifFalseBlockSet(ifFalseBlock);
            }

            instructionContext.FlagsRead |= flags;

            expressions.Add((ExpressionGroup.Conditional, new InstructionCondition(
                Expression.GreaterThan(
                    Expression.And(
                        Expression.Convert(instructionContext.FlagsVariable, typeof(byte)),
                        Expression.Constant((byte)flags, typeof(byte))
                    ),
                    Expression.Constant((byte)0)
                ),
                ifTrueBlock,
                ifFalseBlock
            )));

            return this;
        }

        public InstructionBlock IfFlagsUnset(ProcessorFlags flags, Action<InstructionBlock> ifTrueBlockSet, Action<InstructionBlock> ifFalseBlockSet = null)
        {
            InstructionBlock ifTrueBlock, ifFalseBlock = null;

            ifTrueBlock = new InstructionBlock(context, instructionContext);
            ifTrueBlockSet(ifTrueBlock);

            if (ifFalseBlockSet != null)
            {
                ifFalseBlock = new InstructionBlock(context, instructionContext);
                ifFalseBlockSet(ifFalseBlock);
            }

            instructionContext.FlagsRead |= flags;

            expressions.Add((ExpressionGroup.Conditional, new InstructionCondition(
                Expression.Equal(
                    Expression.And(
                        Expression.Convert(instructionContext.FlagsVariable, typeof(byte)),
                        Expression.Constant((byte)flags, typeof(byte))
                    ),
                    Expression.Constant((byte)0)
                ),
                ifTrueBlock,
                ifFalseBlock
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