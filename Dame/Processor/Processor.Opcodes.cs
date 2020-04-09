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
            public Expression<InstructionValue<T>> ExtraInput { get; set; }
            public Expression<InstructionFunction<T>> Output { get; set; }

            public Mapping(int cycles,
                int opcode,
                string mnemonic,
                string outputName = null,
                Expression<InstructionFunction<T>> output = null,
                string inputName = null,
                Expression<InstructionValue<T>> input = null,
                Expression<InstructionValue<T>> extraInput = null)
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
                ExtraInput = extraInput;
            }
        }

        #endregion

        private void MapOpcodes()
        {
            var vars = new InstructionVariableManager();

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
                    .Input(vars.Get<byte>("VAL8"), mapping.Input)
                    .Output(vars.Get<byte>("VAL8"), mapping.Output)
                    .Compile();
            }

            // LD (HL+), A
            this.opcodes[0x22] = new InstructionBuilder(0x22, "LD (HL+), A", 8)
                    .Input(vars.Get<byte>("VAL8"), () => registers.A)
                    .Output(vars.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val))
                    .Input(vars.Get<byte>("HL"), () => registers.HL)
                    .Add<byte, byte>(vars.Get<byte>("HL"), 1)
                    .Output(vars.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD (HL-), A
            this.opcodes[0x32] = new InstructionBuilder(0x32, "LD (HL-), A", 8)
                    .Input(vars.Get<byte>("VAL8"), () => registers.A)
                    .Output(vars.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val))
                    .Input(vars.Get<byte>("HL"), () => registers.HL)
                    .Subtract<byte>(vars.Get<byte>("HL"), 1)
                    .Output(vars.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD A, (HL+)
            this.opcodes[0x2A] = new InstructionBuilder(0x2A, "LD A, (HL+)", 8)
                    .Input(vars.Get<byte>("VAL8"), () => memoryController.Read(registers.HL))
                    .Output(vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Input(vars.Get<byte>("HL"), () => registers.HL)
                    .Add<byte, byte>(vars.Get<byte>("HL"), 1)
                    .Output(vars.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();
            
            // LD A, (HL-)
            this.opcodes[0x3A] = new InstructionBuilder(0x3A, "LD A, (HL-)", 8)
                    .Input(vars.Get<byte>("VAL8"), () => memoryController.Read(registers.HL))
                    .Output(vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Input(vars.Get<byte>("HL"), () => registers.HL)
                    .Subtract<byte>(vars.Get<byte>("HL"), 1)
                    .Output(vars.Get<byte>("HL"), (byte hl) => registers.SetHL(hl))
                    .Compile();

            #endregion

            #region 16-bit transfers

            var load16Mappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(12, 0x01, "LD", "BC", val => registers.SetBC(val), "d16", () => assembly.ReadDouble()),
                new Mapping<ushort>(12, 0x11, "LD", "DE", val => registers.SetDE(val), "d16", () => assembly.ReadDouble()),
                new Mapping<ushort>(12, 0x21, "LD", "HL", val => registers.SetHL(val), "d16", () => assembly.ReadDouble()),
                new Mapping<ushort>(12, 0x31, "LD", "SP", val => registers.SetSP(val), "d16", () => assembly.ReadDouble()),

                new Mapping<ushort>(20, 0x08, "LD", "(a16)", val => memoryController.WriteDouble(assembly.ReadDouble(), val), "SP", () => registers.SP),

                new Mapping<ushort>(8, 0xF9, "LD", "SP", val => registers.SetSP(val), "HL", () => registers.HL),
            };

            foreach (var mapping in load16Mappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input  (vars.Get<ushort>("VAL16"), mapping.Input)
                    .Output (vars.Get<ushort>("VAL16"), mapping.Output)
                    .Compile();
            }

            // LD HL, SP + r8
            this.opcodes[0xF8] = new InstructionBuilder(0xF8, "LD HL, SP + r8", 12)
                    .Input              (vars.Get<ushort>("VAL16"), () => registers.SP)
                    .Add<ushort, byte>  (vars.Get<ushort>("VAL16"), () => assembly.Read())
                    .UnsetFlags         (ProcessorFlags.Zero | ProcessorFlags.Arithmetic)
                    .ReadFlags          (flags => registers.SetFlags(flags))
                    .Output             (vars.Get<ushort>("VAL16"), (ushort val) => registers.SetHL(val))
                    .Compile();

            var pushMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(16, 0xC5, "PUSH", null, null, "BC", () => registers.BC),
                new Mapping<ushort>(16, 0xD5, "PUSH", null, null, "DE", () => registers.DE),
                new Mapping<ushort>(16, 0xE5, "PUSH", null, null, "HL", () => registers.HL),
                new Mapping<ushort>(16, 0xF5, "PUSH", null, null, "AF", () => registers.AF),
            };

            foreach (var mapping in pushMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                    .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                    .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                    .Input              (vars.Get<ushort>("PTR16"), mapping.Input)
                    .Output             (vars.Get<ushort>("PTR16"), (ushort val) => memoryController.WriteDouble(registers.SP, val))
                    .Compile();
            }

            var popMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(12, 0xC1, "POP", "BC", val => registers.SetBC(val)),
                new Mapping<ushort>(12, 0xD1, "POP", "DE", val => registers.SetDE(val)),
                new Mapping<ushort>(12, 0xE1, "POP", "HL", val => registers.SetHL(val)),
                new Mapping<ushort>(12, 0xF1, "POP", "AF", val => registers.SetAF(val)),
            };

            foreach (var mapping in popMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input              (vars.Get<ushort>("PTR16"), () => memoryController.ReadDouble(registers.SP))
                    .Output             (vars.Get<ushort>("PTR16"), mapping.Output)
                    .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                    .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                    .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                    .Compile();
            }

            #endregion

            #region 8-bit arithmetic

            var incMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x04, "INC", "B", val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(4, 0x0C, "INC", "C", val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(4, 0x14, "INC", "D", val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(4, 0x1C, "INC", "E", val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(4, 0x24, "INC", "H", val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(4, 0x2C, "INC", "L", val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(12, 0x34, "INC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x3C, "INC", "A", val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in incMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), mapping.Input)
                    .Add<byte, byte>(vars.Get<byte>("VAL8"), 1)
                    .ReadFlags      (flags => registers.SetFlags(flags, ProcessorFlags.Zero | ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry))
                    .Output         (vars.Get<byte>("VAL8"), mapping.Output)
                    .Compile();
            }

            var decMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x05, "DEC", "B", val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(4, 0x0D, "DEC", "C", val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(4, 0x15, "DEC", "D", val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(4, 0x1D, "DEC", "E", val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(4, 0x25, "DEC", "H", val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(4, 0x2D, "DEC", "L", val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(12, 0x35, "DEC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x3D, "DEC", "A", val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in decMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), mapping.Input)
                    .Subtract<byte> (vars.Get<byte>("VAL8"), 1)
                    .ReadFlags      (flags => registers.SetFlags(flags, ProcessorFlags.Zero | ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry))
                    .Output         (vars.Get<byte>("VAL8"), mapping.Output)
                    .Compile();
            }

            var addMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x80, "ADD", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0x81, "ADD", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0x82, "ADD", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0x83, "ADD", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0x84, "ADD", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0x85, "ADD", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0x86, "ADD", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x87, "ADD", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xC6, "ADD", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in addMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags      (flags => registers.SetFlags(flags))
                    .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var addCarryMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x88, "ADC", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0x89, "ADC", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0x8A, "ADC", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0x8B, "ADC", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0x8C, "ADC", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0x8D, "ADC", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0x8E, "ADC", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x8F, "ADC", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xCE, "ADC", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in addCarryMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .WriteFlags     (() => registers.Flags)
                    .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input, true)
                    .ReadFlags      (flags => registers.SetFlags(flags))
                    .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var subMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x90, "SUB", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0x91, "SUB", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0x92, "SUB", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0x93, "SUB", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0x94, "SUB", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0x95, "SUB", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0x96, "SUB", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x97, "SUB", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xD6, "SUB", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in subMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .Subtract<byte> (vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags      (flags => registers.SetFlags(flags))
                    .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var subCarryMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0x98, "SBC", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0x99, "SBC", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0x9A, "SBC", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0x9B, "SBC", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0x9C, "SBC", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0x9D, "SBC", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0x9E, "SBC", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0x9F, "SBC", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xDE, "SBC", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in subCarryMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .WriteFlags     (() => registers.Flags)
                    .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input, true)
                    .ReadFlags      (flags => registers.SetFlags(flags))
                    .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var andMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0xA0, "AND", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0xA1, "AND", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0xA2, "AND", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0xA3, "AND", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0xA4, "AND", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0xA5, "AND", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0xA6, "AND", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0xA7, "AND", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xE6, "AND", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in andMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .And<byte>  (vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags  (flags => registers.SetFlags(flags))
                    .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var xorMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0xA8, "XOR", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0xA9, "XOR", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0xAA, "XOR", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0xAB, "XOR", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0xAC, "XOR", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0xAD, "XOR", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0xAE, "XOR", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0xAF, "XOR", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xEE, "XOR", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in xorMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .Xor<byte>  (vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags  (flags => registers.SetFlags(flags))
                    .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var orMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0xB0, "OR", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0xB1, "OR", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0xB2, "OR", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0xB3, "OR", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0xB4, "OR", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0xB5, "OR", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0xB6, "OR", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0xB7, "OR", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xF6, "OR", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in orMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .Or<byte>   (vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags  (flags => registers.SetFlags(flags))
                    .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Compile();
            }

            var cpMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(4, 0xB8, "CP", "A", null, "B", () => registers.B),
                new Mapping<byte>(4, 0xB9, "CP", "A", null, "C", () => registers.C),
                new Mapping<byte>(4, 0xBA, "CP", "A", null, "D", () => registers.D),
                new Mapping<byte>(4, 0xBB, "CP", "A", null, "E", () => registers.E),
                new Mapping<byte>(4, 0xBC, "CP", "A", null, "H", () => registers.H),
                new Mapping<byte>(4, 0xBD, "CP", "A", null, "L", () => registers.L),
                new Mapping<byte>(8, 0xBE, "CP", "A", null, "(HL)", () => memoryController.Read(registers.HL)),
                new Mapping<byte>(4, 0xBF, "CP", "A", null, "A", () => registers.A),

                new Mapping<byte>(8, 0xFE, "CP", "A", null, "d8", () => assembly.Read()),
            };

            foreach (var mapping in cpMappings)
            {
                this.opcodes[mapping.Opcode] = new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, mapping.Cycles)
                    .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .Subtract<byte> (vars.Get<byte>("VAL8"), mapping.Input)
                    .ReadFlags      (flags => registers.SetFlags(flags))
                    .Compile();
            }

            #endregion
        }
    }
}