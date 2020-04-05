using Dame.Memory;

namespace Dame.Processor
{
    sealed class Processor
    {
        private EmulationState currentState;
        private MemoryController<byte> memoryController;
    }
}