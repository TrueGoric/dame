using System;
using Xunit;
using Moq;
using Dame.Emulator.Graphics;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;
using Dame.Emulator.Architecture;
using Dame.Emulator.Graphics.Rendering;
using Dame.Emulator.Memory.Blocks;
using System.Text;
using Dame.Emulator.Cartridges;
using System.IO;
using System.Diagnostics;

namespace Dame.Tests
{
    public class Blargg
    {
        public Blargg()
        { }

        private void RunProgram(string romPath, StringBuilder output, int timeoutSeconds = 30)
        {
            var mockRenderer = new Mock<IRenderer>();
            mockRenderer.DefaultValue = DefaultValue.Mock;
            mockRenderer.SetupAllProperties();

            var context = new ProcessorExecutionContext();

            var registers = new RegisterBank();
            var memory = new MemoryController(0x10000, silenceErrors: true);
            var processor = new Processor(registers, memory, context);
            var graphics = new Graphics(mockRenderer.Object);

            ICartridge testCart;

            using (var testRom = File.Open(romPath, FileMode.Open, FileAccess.Read))
                testCart = CartridgeLoader.Load(testRom);
                
            var ram = new RandomAccessMemoryBlock(0x2000);
            var hRam = new RandomAccessMemoryBlock(0x007F);

            var tempSerial = new RandomAccessMemoryBlock(0x0002);
            
            memory.AddBlock(0xC000..0xE000, ram);
            memory.AddBlock(0xFF01..0xFF03, tempSerial);
            memory.AddBlock(0xFF80..0xFFFF, hRam);

            testCart.RegisterBlocks(memory);
            graphics.RegisterBlocks(memory);

            var termBuffer = new StringBuilder();
            var timeoutWatch = Stopwatch.StartNew();

            while (true)
            {
                processor.Step();

                if (memory.Read(0xFF02) == 0x81) // temp serial
                {
                    var val = (char)memory.Read(0xFF01);

                    Console.Write(val);
                    termBuffer.Append(val);
                    
                    if (val == '\n')
                    {
                        var line = termBuffer.ToString();
                        var lineParsed = line.Trim().ToLower();
                        
                        output.Append(line);

                        if (lineParsed == "passed" || lineParsed == "failed")
                            return;
                        
                        termBuffer.Clear();
                    }

                    memory.Write(0xFF02, 0x01);
                }

                if (timeoutWatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                {
                    output.Append(termBuffer.ToString());
                    return;
                }
            }
        }

        [Fact]
        public void CPU_01_Special()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/01-special.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_02_Interrupts()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/02-interrupts.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_03_OpSPHL()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/03-op sp,hl.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_04_OpRegVal()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/04-op r,imm.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_05_OpReg16()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/05-op rp.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_06_Load()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/06-ld r,r.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_07_JumpsCallsRsts()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/07-jr,jp,call,ret,rst.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_08_Miscellaneous()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/08-misc instrs.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_09_OpRegReg()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/09-op r,r.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_10_BitOps()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/10-bit ops.gb", output);

            Assert.Contains("Passed", output.ToString());
        }

        [Fact]
        public void CPU_11_OpAHL()
        {
            var output = new StringBuilder();

            RunProgram(@"Binaries/Blargg/ProcessorInstructions/11-op a,(hl).gb", output);

            Assert.Contains("Passed", output.ToString());
        }
    }
}
