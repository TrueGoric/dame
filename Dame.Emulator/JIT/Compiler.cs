using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Dame.Emulator.Exceptions;
using Dame.Emulator.Instructions;
using Dame.Emulator.Processor;
using static Dame.Emulator.JIT.CodeFactory;

namespace Dame.Emulator.JIT
{
    public sealed partial class Compiler
    {
        public bool PrintDisassemblyOnExecution { get; set; }
        public int InstructionsPerBlock { get; set; }

        public unsafe Instruction Compile(ushort opcode)
        {
            var alloc = stackalloc byte[sizeof(ushort)];
            var span = new Span<byte>(alloc, sizeof(ushort));
            var ushortSpan = MemoryMarshal.Cast<byte, ushort>(span);

            ushortSpan[0] = opcode;

            var method = CreateInstructionMethod("Instr");
            var gen = method.GetILGenerator();

            var (_, cycles, name) = Interpret(gen, span, true);

            var invoker = (InstructionMethod)method.CreateDelegate(typeof(InstructionMethod));

            return new Instruction(opcode, name, cycles, invoker);
        }

        public JITBlock Compile(ReadOnlySpan<byte> assembly)
        {
            throw new NotImplementedException();
        }

        private (int, byte, string) Interpret(ILGenerator gen, ReadOnlySpan<byte> assembly, bool interpreted)
        {
            var isPrefixed = false;
            var bytesConsumed = (byte)0;
            var cycles = (byte)1;
            var name = "";
            
            var firstByte = assembly[0];

            Register registerOne, registerTwo;

            switch (firstByte)
            {
                case 0x00: name = "NOP"; break;

                // LD
                case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x47: // LD B, r
                case 0x48: case 0x49: case 0x4A: case 0x4B: case 0x4C: case 0x4D: case 0x4F: // LD C, r
                case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x57: // LD D, r
                case 0x58: case 0x59: case 0x5A: case 0x5B: case 0x5C: case 0x5D: case 0x5F: // LD E, r
                case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x67: // LD H, r
                case 0x68: case 0x69: case 0x6A: case 0x6B: case 0x6C: case 0x6D: case 0x6F: // LD L, r
                case 0x78: case 0x79: case 0x7A: case 0x7B: case 0x7C: case 0x7D: case 0x7F: // LD A, r
                    registerOne = (Register)((firstByte - 0x40) / 7);
                    registerTwo = (Register)(firstByte % 7);

                    cycles = 1;
                    name = $"LD {registerOne.GetName()}, {registerTwo.GetName()}";

                    gen.LoadRegisterToStack(registerTwo)
                        .WriteStackToRegister(registerOne);
                    break;
                
                case 0x46: case 0x4E: case 0x56: case 0x5E: case 0x66: case 0x6E: case 0x7E: // LD r, (HL)
                    registerOne = (Register)((firstByte - 0x40) / 7);

                    cycles = 2;
                    name = $"LD {registerOne.GetName()}, ({Register.HL.GetName()})";

                    gen.LoadRegisterToStack(Register.HL)
                        .LoadMemoryToStack()
                        .WriteStackToRegister(registerOne);
                    break;
                
                case 0x70: case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x77: // LD (HL), r
                    registerTwo = (Register)(firstByte % 7);

                    cycles = 2;
                    name = $"LD ({Register.HL.GetName()}), {registerTwo.GetName()}";

                    gen.LoadRegisterToStack(Register.HL)
                        .LoadMemoryToStack()
                        .LoadRegisterToStack(registerTwo)
                        .WriteStackToMemory();
                    break;

                case 0xCB: isPrefixed = true; break;

                default:
                    throw new InstructionNotImplementedException(firstByte);
            }

            bytesConsumed += 1;

            if (isPrefixed)
            {
                var secondByte = assembly[1];

                switch (secondByte)
                {
                    
                    default:
                        throw new InstructionNotImplementedException(firstByte << 4 + secondByte);
                }

                bytesConsumed += 1;
            }

            return (bytesConsumed, cycles, name);
        }
    }
}
