using System;

namespace Dame
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new Architecture.ProcessorExecutionContext();
            var state = new EmulationState(new byte[0xFFFF]);
            var memory = new Memory.MemoryController(0xFFFF);
            
            var processor = new Processor.Processor(state, memory, context);
        }
    }
}
