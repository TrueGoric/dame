using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dame.Accessors;
using Dame.Architecture;
using Dame.Memory;
using Dame.Processor;

namespace Dame.Instructions
{
    sealed class InstructionBuilder
    {
        #region InstructionContext

        private class InstructionContext : IInstructionContext
        {
            public ProcessorFlags FlagsRead { get; set; } = ProcessorFlags.None;

            public ParameterExpression FlagsVariable { get; }

            public InstructionContext()
                : this(Expression.Variable(typeof(byte), "FLAGS"))
            {}

            public InstructionContext(ParameterExpression flagsVar)
            {
                FlagsVariable = flagsVar;
            }
        }

        #endregion
        private int opcode;
        private string mnemonic;
        private readonly ProcessorExecutionContext context;

        private List<(ExpressionGroup Group, Expression Expr)> expressions;

        public InstructionBuilder(int opcode, string mnemonic, ProcessorExecutionContext context)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.context = context;

            expressions = new List<(ExpressionGroup Group, Expression Expr)>();
        }

        public InstructionBuilder With(Action<InstructionBlock> blockSet)
        {
            var instructionContext = new InstructionContext();
            var block = new InstructionBlock(context, instructionContext);

            blockSet(block);

            expressions.AddRange(block.Expressions);

            return this;
        }

        public Instruction Compile()
        {
            throw new NotImplementedException();
        }
    }
}