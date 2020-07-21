using System;
using Dame.Emulator.Instructions;
using Dame.Emulator.JIT;

namespace Dame.Emulator.Processor
{
    public sealed partial class Processor
    {
        private void MapOpcodes()
        {
            var compiler = new Compiler();

            opcodes.Add(0x00, compiler.Compile(0x00));
        }
    }
}
