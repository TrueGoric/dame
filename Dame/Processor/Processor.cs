using System;
using System.Collections.Generic;
using Dame.Accessors;
using Dame.Architecture;
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
        private MemoryAccessor assembly;

        private Dictionary<int, Instruction> opcodes;

        public Processor(EmulationState state, MemoryController memory, ProcessorExecutionContext context)
        {
            currentState = state;
            memoryController = memory;
            cpuContext = context;

            registers = new RegisterAccessor(currentState);
            assembly = new MemoryAccessor(memoryController);

            opcodes = new Dictionary<int, Instruction>();

            MapOpcodes();
        }
    }
}