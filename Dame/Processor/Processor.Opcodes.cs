using System;
using Dame.Accessors;
using Dame.Memory;
using Dame.Instructions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dame.Processor
{
    sealed partial class Processor
    {
        #region Mapping<T> helper class

        private class Mapping<T>
            where T : unmanaged
        {
            public int Cycles { get; set; }
            public string Mnemonic { get; set; }

            public int Opcode { get; set; }

            public Expression<InstructionValue<T>> Input { get; set; }
            public Expression<InstructionFunction<T>> Output { get; set; }

            public Mapping(int cycles,
                int opcode,
                string mnemonic,
                string outputName = null,
                Expression<InstructionFunction<T>> output = null,
                string inputName = null,
                Expression<InstructionValue<T>> input = null)
            {
                Cycles = cycles;
                Opcode = opcode;

                Mnemonic = mnemonic;

                if (outputName != null)
                    Mnemonic += outputName;
                if (outputName != null && inputName != null)
                    Mnemonic += ", ";
                if (inputName != null)
                    Mnemonic += inputName;

                Input = input;
                Output = output;
            }
        }

        #endregion

        private void MapOpcodes()
        {
            var variables = new InstructionVariableManager();

            #region 8-bit loads

            // TODO: maybe use alternative mnemonics?

            var load8Mappings = new Mapping<byte>[] // this is gonna hurt
            {
                new Mapping<byte>(8, 0x02, "LD", "(BC)", val => memoryController.Write(registers.BC, val), "A", () => registers.A),
                new Mapping<byte>(8, 0x12, "LD", "(DE)", val => memoryController.Write(registers.DE, val), "A", () => registers.A),
                new Mapping<byte>(8, 0x0A, "LD", "A", val => registers.SetA(val), "(BC)", () => memoryController.Read(registers.BC)),
                new Mapping<byte>(8, 0x1A, "LD", "A", val => registers.SetA(val), "(DE)", () => memoryController.Read(registers.DE)),
                
                new Mapping<byte>(8, 0x06, "LD", "B", val => registers.SetB(val), "d8", () => assembly.Read()),
                new Mapping<byte>(8, 0x16, "LD", "D", val => registers.SetD(val), "d8", () => assembly.Read()),
                new Mapping<byte>(8, 0x26, "LD", "H", val => registers.SetH(val), "d8", () => assembly.Read()),
                new Mapping<byte>(12, 0x36, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "d8", () => assembly.Read()),

                new Mapping<byte>(8, 0x0E, "LD", "C", val => registers.SetC(val), "d8", () => assembly.Read()),
                new Mapping<byte>(8, 0x1E, "LD", "E", val => registers.SetE(val), "d8", () => assembly.Read()),
                new Mapping<byte>(8, 0x2E, "LD", "L", val => registers.SetL(val), "d8", () => assembly.Read()),
                new Mapping<byte>(8, 0x3E, "LD", "A", val => registers.SetA(val), "d8", () => assembly.Read()),

                new Mapping<byte>(4, 0x40, "LD", "B", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x41, "LD", "B", val => registers.SetB(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x42, "LD", "B", val => registers.SetB(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x43, "LD", "B", val => registers.SetB(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x44, "LD", "B", val => registers.SetB(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x45, "LD", "B", val => registers.SetB(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x46, "LD", "B", val => registers.SetB(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x47, "LD", "B", val => registers.SetB(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x48, "LD", "C", val => registers.SetC(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x49, "LD", "C", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x4A, "LD", "C", val => registers.SetC(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x4B, "LD", "C", val => registers.SetC(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x4C, "LD", "C", val => registers.SetC(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x4D, "LD", "C", val => registers.SetC(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x4E, "LD", "C", val => registers.SetC(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x4F, "LD", "C", val => registers.SetC(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x50, "LD", "D", val => registers.SetD(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x51, "LD", "D", val => registers.SetD(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x52, "LD", "D", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x53, "LD", "D", val => registers.SetD(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x54, "LD", "D", val => registers.SetD(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x55, "LD", "D", val => registers.SetD(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x56, "LD", "D", val => registers.SetD(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x57, "LD", "D", val => registers.SetD(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x58, "LD", "E", val => registers.SetE(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x59, "LD", "E", val => registers.SetE(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x5A, "LD", "E", val => registers.SetE(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x5B, "LD", "E", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x5C, "LD", "E", val => registers.SetE(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x5D, "LD", "E", val => registers.SetE(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x5E, "LD", "E", val => registers.SetE(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x5F, "LD", "E", val => registers.SetE(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x60, "LD", "H", val => registers.SetH(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x61, "LD", "H", val => registers.SetH(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x62, "LD", "H", val => registers.SetH(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x63, "LD", "H", val => registers.SetH(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x64, "LD", "H", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x65, "LD", "H", val => registers.SetH(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x66, "LD", "H", val => registers.SetH(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x67, "LD", "H", val => registers.SetH(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x68, "LD", "L", val => registers.SetL(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x69, "LD", "L", val => registers.SetL(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x6A, "LD", "L", val => registers.SetL(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x6B, "LD", "L", val => registers.SetL(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x6C, "LD", "L", val => registers.SetL(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x6D, "LD", "L", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x6E, "LD", "L", val => registers.SetL(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x6F, "LD", "L", val => registers.SetL(val), "A", () => registers.A),

                new Mapping<byte>(4, 0x70, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "B", () => registers.B),
                new Mapping<byte>(4, 0x71, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "C", () => registers.C),
                new Mapping<byte>(4, 0x72, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "D", () => registers.D),
                new Mapping<byte>(4, 0x73, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "E", () => registers.E),
                new Mapping<byte>(4, 0x74, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "H", () => registers.H),
                new Mapping<byte>(4, 0x75, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "L", () => registers.L),
                new Mapping<byte>(4, 0x77, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "A", () => registers.A),

                new Mapping<byte>(4, 0x78, "LD", "A", val => registers.SetA(val), "B", () => registers.B),
                new Mapping<byte>(4, 0x79, "LD", "A", val => registers.SetA(val), "C", () => registers.C),
                new Mapping<byte>(4, 0x7A, "LD", "A", val => registers.SetA(val), "D", () => registers.D),
                new Mapping<byte>(4, 0x7B, "LD", "A", val => registers.SetA(val), "E", () => registers.E),
                new Mapping<byte>(4, 0x7C, "LD", "A", val => registers.SetA(val), "H", () => registers.H),
                new Mapping<byte>(4, 0x7D, "LD", "A", val => registers.SetA(val), "L", () => registers.L),
                new Mapping<byte>(8, 0x7E, "LD", "A", val => registers.SetA(val), "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x7F, "LD", "A", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(12, 0xE0, "LDH", "(a8)", val => memoryController.Write(0xFF00 + assembly.Read(), val), "A", () => registers.A),
                new Mapping<byte>(12, 0xF0, "LDH", "A", val => registers.SetA(val), "(a8)", () => memoryController.Read(0xFF00 + assembly.Read())),

                new Mapping<byte>(8, 0xE2, "LD", "(C)", val => memoryController.Write(0xFF00 + registers.C, val), "A", () => registers.A),
                new Mapping<byte>(8, 0xF2, "LD", "A", val => registers.SetA(val), "(a8)", () => memoryController.Read(0xFF00 + registers.C)),

                new Mapping<byte>(16, 0xEA, "LD", "(a16)", val => memoryController.Write(assembly.ReadDouble(), val), "A", () => registers.A),
                new Mapping<byte>(16, 0xFA, "LD", "A", val => registers.SetA(val), "(a16)", () => memoryController.Read(assembly.ReadDouble())),
            };

            foreach (var mapping in load8Mappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input(variables.Get<byte>("VAL8"), mapping.Input)
                    .Output(variables.Get<byte>("VAL8"), mapping.Output)
                    .Compile();
            }

            // LD (HL+), A
            this.opcodes[0x22] = new InstructionBuilder(0x22, "LD (HL+), A", 8)
                    .Input(variables.Get<byte>("VAL8"), () => registers.A)
                    .Output(variables.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val))
                    .Input(variables.Get<byte>("HL"), () => registers.HL)
                    .Add<byte, byte>(variables.Get<byte>("HL"), () => 1)
                    .Output(variables.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD (HL-), A
            this.opcodes[0x32] = new InstructionBuilder(0x32, "LD (HL-), A", 8)
                    .Input(variables.Get<byte>("VAL8"), () => registers.A)
                    .Output(variables.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val))
                    .Input(variables.Get<byte>("HL"), () => registers.HL)
                    .Subtract<byte>(variables.Get<byte>("HL"), () => 1)
                    .Output(variables.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD A, (HL+)
            this.opcodes[0x2A] = new InstructionBuilder(0x2A, "LD A, (HL+)", 8)
                    .Input(variables.Get<byte>("VAL8"), () => memoryController.Read(registers.HL))
                    .Output(variables.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Input(variables.Get<byte>("HL"), () => registers.HL)
                    .Add<byte, byte>(variables.Get<byte>("HL"), () => 1)
                    .Output(variables.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD A, (HL-)
            this.opcodes[0x3A] = new InstructionBuilder(0x3A, "LD A, (HL-)", 8)
                    .Input(variables.Get<byte>("VAL8"), () => memoryController.Read(registers.HL))
                    .Output(variables.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Input(variables.Get<byte>("HL"), () => registers.HL)
                    .Subtract<byte>(variables.Get<byte>("HL"), () => 1)
                    .Output(variables.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();

            #endregion
        }
    }
}