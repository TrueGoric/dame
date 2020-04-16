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

        
    }
}