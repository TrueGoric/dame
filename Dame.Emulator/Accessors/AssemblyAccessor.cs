using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Accessors
{
    public sealed class AssemblyAccessor : MemoryAccessor
    {
        private readonly RegisterBank registerAccessor;

        public override int Location
        {
            get => registerAccessor.PC;
            set => registerAccessor.PC = (ushort)value; // TODO: overflow check?
        }

        public AssemblyAccessor(MemoryController controller, RegisterBank registers)
            : base(controller)
        {
            registerAccessor = registers;
        }
    }
}