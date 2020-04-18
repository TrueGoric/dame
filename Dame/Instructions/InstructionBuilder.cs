using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<InstructionBlock> blocks;

        public InstructionBuilder(int opcode, string mnemonic, ProcessorExecutionContext context)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.context = context;

            blocks = new List<InstructionBlock>();
        }

        public InstructionBuilder With(Action<InstructionBlock> blockSet)
        {
            var instructionContext = new InstructionContext();
            var block = new InstructionBlock(context, instructionContext);

            blockSet(block);

            blocks.Add(block);

            return this;
        }

        public Instruction Compile()
        {
            var expressions = blocks
                .SelectMany(b => b.Expressions)
                .ToList();

            var variables = blocks
                .SelectMany(b => b.Variables)
                .Distinct()
                .ToList();
            
            var statements = expressions
                .Select(e => e.Expr);

            var prefab = Expression.Lambda<InstructionDelegate>(
                Expression.Block(
                    variables,
                    statements
                ), mnemonic, null
            );

            var @delegate = prefab
#if DEBUG
            .Compile();
#else
            .CompileFast();
#endif
            return new Instruction(mnemonic, @delegate);
        }
    }
}