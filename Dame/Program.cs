using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dame.Emulator;
using Dame.Emulator.Graphics;
using Dame.Emulator.Memory;
using Dame.Emulator.Memory.Blocks;

namespace Dame
{
    class Program
    {
        static void Main(string[] args)
        {
            var dpiMultiplier = 3u;
            var window = new EmulatorWindow(dpiMultiplier);

            var context = new Emulator.Architecture.ProcessorExecutionContext();
            var state = new EmulationState(new byte[0x10000]);

            var memory = new MemoryController(0x10000);
            var processor = new Emulator.Processor.Processor(state, memory, context);
            var graphics = new Graphics(window.EmulatorRenderer);

            context.Register(graphics);

            var windowTask = window.RunAsync();

            var bootRaw = new byte[] {}; // 0x4EDAC7ED

            var logo = new byte[] {}; // 0x4EDAC7ED

            var bootSpan = new Span<byte>(state.Memory, 0x0, 0x0100);
            bootRaw.CopyTo(bootSpan);

            var logoSpan = new Span<byte>(state.Memory, 0x0104, 0x30);
            logo.CopyTo(logoSpan);

            var bootRom = new ReadOnlyMemoryBlock(0x0100);
            var tempRom = new ReadOnlyMemoryBlock(0x7F00);

            var ram = new RandomAccessMemoryBlock(0x2000);
            var hRam = new RandomAccessMemoryBlock(0x007F);

            var tempSound = new RandomAccessMemoryBlock(0x0017);
            
            memory.AddBlock(0x0..0x0100, bootRom);
            memory.AddBlock(0x0100..0x8000, tempRom);
            memory.AddBlock(0xC000..0xE000, ram);
            memory.AddBlock(0xFF10..0xFF27, tempSound);
            memory.AddBlock(0xFF80..0xFFFF, hRam);
            
            graphics.RegisterBlocks(memory);
            
            // for (int i = 0x104; i < 0x133; i++)
            //     state.Memory[i] = 0xCE;

            var count = 0L;
            var watch = Stopwatch.StartNew();

            while (true)
            {
                processor.Step();

                if (watch.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine($"{(context.Ticks - count) / (1000d * 1000d)}MHz");
                    count = context.Ticks;
                    watch.Restart();
                }
            }
        }
    }
}
