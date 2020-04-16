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
        private int opcode;
        private string mnemonic;
        private readonly ProcessorExecutionContext context;

        private List<(ExpressionGroup Group, Expression Expr)> expressions;

        public InstructionBuilder(int opcode, string mnemonic, ProcessorExecutionContext context)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.context = context;
        }

        public InstructionBuilder With(Action<InstructionBlock> blockSet)
        {
            var block = new InstructionBlock(context);

            blockSet(block);

            expressions.AddRange(block.Expressions);

            // TODO: pre-optimize flags if not read

            return this;
        }

        public Instruction Compile()
        {
            throw new NotImplementedException();
        }
    }
}