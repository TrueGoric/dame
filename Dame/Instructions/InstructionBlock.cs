using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Architecture;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed partial class InstructionBlock
    {
        #region InstructionCondition class

        private class InstructionCondition
        {
            public Expression Condition { get; set; }

            public InstructionBlock IfTrue { get; set; }
            public InstructionBlock IfFalse { get; set; }

            public InstructionCondition(Expression condition, InstructionBlock ifTrue)
            {
                Condition = condition;
                IfTrue = ifTrue;
            }

            public InstructionCondition(Expression condition, InstructionBlock ifTrue, InstructionBlock ifFalse)
                : this(condition, ifTrue)
            {
                IfFalse = ifFalse;
            }
        }

        #endregion

        private readonly ProcessorExecutionContext context;
        private readonly IInstructionContext instructionContext;

        private List<(ExpressionGroup Group, object Expr)> expressions;
        private List<ParameterExpression> variables;

        public IEnumerable<(ExpressionGroup Group, Expression Expr)> Expressions => PopulateExpressions(this);
        public IEnumerable<ParameterExpression> Variables => PopulateVariables(this);

        public ProcessorFlags FlagsRead => instructionContext.FlagsRead;

        public InstructionBlock(ProcessorExecutionContext context, IInstructionContext instructionContext)
        {
            this.context = context;
            this.instructionContext = instructionContext;

            expressions = new List<(ExpressionGroup Group, object Expr)>();
            variables = new List<ParameterExpression>();

            variables.Add(instructionContext.FlagsVariable);
        }

        private static IEnumerable<(ExpressionGroup Group, Expression Expr)> PopulateExpressions(InstructionBlock block)
        {
            foreach (var expr in block.expressions)
            {
                if (expr.Expr is InstructionBlock nestedBlock)
                {
                    foreach (var nestedExpr in PopulateExpressions(nestedBlock))
                        yield return nestedExpr;
                }
                else if (expr.Expr is InstructionCondition nestedCondition)
                {
                    yield return (ExpressionGroup.Conditional, GenerateConditionalExpression(nestedCondition));
                }
                else if (expr.Expr is Expression nestedExpr)
                {
                    // TODO: pre-optimize flags if not read
                    
                    yield return (expr.Group, nestedExpr);
                }
            }
        }

        private static IEnumerable<ParameterExpression> PopulateVariables(InstructionBlock block)
        {
            foreach (var variable in block.variables)
                yield return variable;

            foreach (var expr in block.expressions)
            {
                if (expr.Expr is InstructionBlock nestedBlock)
                {
                    foreach (var nestedVariable in PopulateVariables(nestedBlock))
                        yield return nestedVariable;
                }
                else if (expr.Expr is InstructionCondition nestedCondition)
                {
                    foreach (var nestedVariable in PopulateVariables(nestedCondition.IfTrue))
                        yield return nestedVariable;

                    if (nestedCondition.IfFalse != null)
                        foreach (var nestedVariable in PopulateVariables(nestedCondition.IfFalse))
                            yield return nestedVariable;
                }
            }
        }

        private void ThrowOnUnsupportedType<T>()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    return;

                default:
                    throw new NotSupportedException($"Type {typeof(T).FullName} is not supported by {nameof(InstructionBuilder)}!");
            }
        }

        private void ThrowOnVariableTypeMismatch<T>(ParameterExpression variable)
        {
            if (variable.Type != typeof(T))
                throw new ArgumentException("Provided expression is not of the declared generic type!", nameof(variable));
        }
    }
}