using System;
using System.Collections.Generic;
using Dame.Accessors;
using Dame.Architecture;
using Dame.Exceptions;
using Dame.Instructions;
using Dame.Memory;

namespace Dame.Processor
{
    sealed partial class Processor
    {
        private ProcessorExecutionContext cpuContext;
        private EmulationState currentState;
        private MemoryController memoryController;

        private RegisterAccessor registers;
        private AssemblyAccessor assembly;

        private Dictionary<int, Instruction> opcodes;

        public Processor(EmulationState state, MemoryController memory, ProcessorExecutionContext context)
        {
            currentState = state;
            memoryController = memory;
            cpuContext = context;

            registers = new RegisterAccessor(currentState);
            assembly = new AssemblyAccessor(memoryController, registers);

            opcodes = new Dictionary<int, Instruction>();

            MapOpcodes();
        }

        public void Step()
        {
            int opcode;

            if (assembly.Location > 0x0100)
            {
                var l = 0;
            }

            opcode = assembly.Read();

            if (opcode == 0xCB)
            {
                opcode <<= 8;
                opcode += assembly.Read();
            }

            ProcessInstruction(opcode);
        }

        private void ProcessInstruction(int opcode)
        {
            Instruction instruction;

            if (!opcodes.TryGetValue(opcode, out instruction))
                throw new InstructionNotImplementedException(opcode);

            instruction.Invoker();
        }

    }
}