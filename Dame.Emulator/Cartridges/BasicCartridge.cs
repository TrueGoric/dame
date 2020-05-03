using System;
using System.IO;
using Dame.Emulator.Memory;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Cartridges
{
    public class BasicCartridge : ICartridge
    {
        public ReadOnlyMemoryBlock ROM { get; }

        public BasicCartridge(ReadOnlySpan<byte> data)
        {
            ROM = new ReadOnlyMemoryBlock(data.Length);

            data.CopyTo(ROM.Memory);
        }

        public void RegisterBlocks(MemoryController controller)
        {
            controller.AddBlock(0x0..0x8000, ROM);
        }
    }
}
