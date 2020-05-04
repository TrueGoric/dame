using System;
using Dame.Emulator.Accessors;
using Dame.Emulator.Memory;
using Dame.Emulator.Instructions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dame.Emulator.Processor
{
    public sealed partial class Processor
    {
        #region Mapping<T> helper class

        private class Mapping<T>
            where T : unmanaged
        {
            public string Mnemonic { get; set; }

            public int Opcode { get; set; }

            public Expression<InstructionValue<T>> Input { get; set; }
            public Expression<InstructionFunction<T>> Output { get; set; }

            public int InputCycles { get; set; }
            public int OutputCycles { get; set; }

            public Mapping(int opcode,
                string mnemonic,
                string outputName = null,
                Expression<InstructionFunction<T>> output = null,
                string inputName = null,
                Expression<InstructionValue<T>> input = null,
                int inputCycles = 0,
                int outputCycles = 0)
            {
                Opcode = opcode;

                Mnemonic = mnemonic;

                if (outputName != null)
                    Mnemonic += " " + outputName;
                if (outputName != null && inputName != null)
                    Mnemonic += ",";
                if (inputName != null)
                    Mnemonic += " " + inputName;

                Input = input;
                Output = output;
                
                InputCycles = inputCycles;
                OutputCycles = outputCycles;
            }
        }

        #endregion

        private void MapOpcodes()
        {
            // TODO: write tests -_-
            var vars = new InstructionVariableManager();

            #region Control instructions

            // NOP
            this.opcodes.Add(0x00, new InstructionBuilder(0x00, "NOP", cpuContext)
                .With(b => b
                    .Cycle  ())
                .Compile());

            // temp
            this.opcodes.Add(0xF3, new InstructionBuilder(0xF3, "DI", cpuContext)
                .With(b => b
                    .Cycle  ())
                .Compile());
            
            #endregion

            #region Jump instructions

            var jpMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0xC3, "JP", null, null, "a16", () => assembly.ReadDouble(), inputCycles: 3),
                new Mapping<ushort>(0xE9, "JP", null, null, "(HL)", () => registers.HL),
            };

            foreach (var mapping in jpMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input  (vars.Get<ushort>("PTR16"), mapping.Input, mapping.InputCycles)
                        .Output (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                        .Cycle  ())
                    .Compile());
            }

            // JR r8
            this.opcodes.Add(0x18, new InstructionBuilder(0x18, "JR r8", cpuContext)
                .With(b => b
                    .Input              (vars.Get<sbyte>("R8"), () => assembly.Read().TwosComplement())
                    .Input              (vars.Get<ushort>("PTR16"), () => registers.PC)
                    .Add<ushort, sbyte> (vars.Get<ushort>("PTR16"), vars.Get<sbyte>("R8"), setFlags: false)
                    .Cycle              (2)
                    .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                    .Cycle              ())
                .Compile());

            // JP NZ, a16
            this.opcodes.Add(0xC2, new InstructionBuilder(0xC2, "JP NZ, a16", cpuContext)
                .With(b => b
                    .Input          (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .WriteFlags     (() => registers.Flags)
                    .IfFlagsUnset   (ProcessorFlags.Zero,
                        t => t
                            .Output (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle  ())
                    .Cycle          ())
                .Compile());
            
            // JP Z, a16
            this.opcodes.Add(0xCA, new InstructionBuilder(0xCA, "JP Z, a16", cpuContext)
                .With(b => b
                    .Input          (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .WriteFlags     (() => registers.Flags)
                    .IfFlagsSet     (ProcessorFlags.Zero,
                        t => t
                            .Output (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle  ())
                    .Cycle          ())
                .Compile());
            
            // JP NC, a16
            this.opcodes.Add(0xD2, new InstructionBuilder(0xD2, "JP NC, a16", cpuContext)
                .With(b => b
                    .Input          (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .WriteFlags     (() => registers.Flags)
                    .IfFlagsUnset   (ProcessorFlags.Carry,
                        t => t
                            .Output (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle  ())
                    .Cycle          ())
                .Compile());
            
            // JP C, a16
            this.opcodes.Add(0xDA, new InstructionBuilder(0xDA, "JP C, a16", cpuContext)
                .With(b => b
                    .Input          (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .WriteFlags     (() => registers.Flags)
                    .IfFlagsSet     (ProcessorFlags.Carry,
                        t => t
                            .Output (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle  ())
                    .Cycle          ())
                .Compile());
            
            // JR NZ, r8
            this.opcodes.Add(0x20, new InstructionBuilder(0x20, "JR NZ, r8", cpuContext)
                .With(b => b
                    .Input              (vars.Get<sbyte>("R8"), () => assembly.Read().TwosComplement())
                    .Input              (vars.Get<ushort>("PTR16"), () => registers.PC)
                    .Add<ushort, sbyte> (vars.Get<ushort>("PTR16"), vars.Get<sbyte>("R8"), setFlags: false)
                    .Cycle              ()
                    .WriteFlags         (() => registers.Flags)
                    .IfFlagsUnset       (ProcessorFlags.Zero,
                        t => t
                            .Output     (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle      ())
                    .Cycle              ())
                .Compile());
            
            // JR Z, r8
            this.opcodes.Add(0x28, new InstructionBuilder(0x28, "JR Z, r8", cpuContext)
                .With(b => b
                    .Input              (vars.Get<sbyte>("R8"), () => assembly.Read().TwosComplement())
                    .Input              (vars.Get<ushort>("PTR16"), () => registers.PC)
                    .Add<ushort, sbyte> (vars.Get<ushort>("PTR16"), vars.Get<sbyte>("R8"), setFlags: false)
                    .Cycle              ()
                    .WriteFlags         (() => registers.Flags)
                    .IfFlagsSet         (ProcessorFlags.Zero,
                        t => t
                            .Output     (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle      ())
                    .Cycle              ())
                .Compile());
            
            // JR NC, r8
            this.opcodes.Add(0x30, new InstructionBuilder(0x30, "JR NC, r8", cpuContext)
                .With(b => b
                    .Input              (vars.Get<sbyte>("R8"), () => assembly.Read().TwosComplement())
                    .Input              (vars.Get<ushort>("PTR16"), () => registers.PC)
                    .Add<ushort, sbyte> (vars.Get<ushort>("PTR16"), vars.Get<sbyte>("R8"), setFlags: false)
                    .Cycle              ()
                    .WriteFlags         (() => registers.Flags)
                    .IfFlagsUnset       (ProcessorFlags.Carry,
                        t => t
                            .Output     (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle      ())
                    .Cycle              ())
                .Compile());
            
            // JR C, r8
            this.opcodes.Add(0x38, new InstructionBuilder(0x38, "JP C, r8", cpuContext)
                .With(b => b
                    .Input              (vars.Get<sbyte>("R8"), () => assembly.Read().TwosComplement())
                    .Input              (vars.Get<ushort>("PTR16"), () => registers.PC)
                    .Add<ushort, sbyte> (vars.Get<ushort>("PTR16"), vars.Get<sbyte>("R8"), setFlags: false)
                    .Cycle              ()
                    .WriteFlags         (() => registers.Flags)
                    .IfFlagsSet         (ProcessorFlags.Carry,
                        t => t
                            .Output     (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                            .Cycle      ())
                    .Cycle              ())
                .Compile());

            #endregion

            #region Call instructions
            
            // CALL a16
            this.opcodes.Add(0xCD, new InstructionBuilder(0xCD, "CALL a16", cpuContext)
                .With(b => b
                    .Input              (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    // SP -= 2
                    .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                    .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                    .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                    .Cycle              ()
                    // (SP) = PC
                    .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                    .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                    // PC = a16
                    .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                    .Cycle              ())
                .Compile());
            
            // CALL NZ, a16
            this.opcodes.Add(0xC4, new InstructionBuilder(0xC4, "CALL NZ, a16", cpuContext)
                .With(b => b
                    .Input                      (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .IfFlagsUnset               (ProcessorFlags.Zero,
                        t => t
                            // SP -= 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                            .Cycle              ()
                            // (SP) = PC
                            .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                            // PC = a16
                            .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val)))
                    .Cycle                      ())
                .Compile());
            
            // CALL Z, a16
            this.opcodes.Add(0xCC, new InstructionBuilder(0xCC, "CALL Z, a16", cpuContext)
                .With(b => b
                    .Input                      (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .IfFlagsSet                 (ProcessorFlags.Zero,
                        t => t
                            // SP -= 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                            .Cycle              ()
                            // (SP) = PC
                            .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                            // PC = a16
                            .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val)))
                    .Cycle                      ())
                .Compile());
            
            // CALL NC, a16
            this.opcodes.Add(0xD4, new InstructionBuilder(0xD4, "CALL NC, a16", cpuContext)
                .With(b => b
                    .Input                      (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .IfFlagsUnset               (ProcessorFlags.Carry,
                        t => t
                            // SP -= 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                            .Cycle              ()
                            // (SP) = PC
                            .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                            // PC = a16
                            .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val)))
                    .Cycle                      ())
                .Compile());
            
            // CALL C, a16
            this.opcodes.Add(0xDC, new InstructionBuilder(0xDC, "CALL C, a16", cpuContext)
                .With(b => b
                    .Input                      (vars.Get<ushort>("PTR16"), () => assembly.ReadDouble(), 2)
                    .IfFlagsSet                 (ProcessorFlags.Carry,
                        t => t
                            // SP -= 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                            .Cycle              ()
                            // (SP) = PC
                            .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                            // PC = a16
                            .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val)))
                    .Cycle                      ())
                .Compile());

            // RET
            this.opcodes.Add(0xC9, new InstructionBuilder(0xC9, "RET", cpuContext)
                .With(b => b
                    // PC = (SP)
                    .Input              (vars.Get<ushort>("COUNTER16"), () => memoryController.ReadDouble(registers.SP), 2)
                    .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => registers.SetPC(val))
                    // SP += 2
                    .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                    .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                    .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                    .Cycle              (2))
                .Compile());
            
            // RET NZ
            this.opcodes.Add(0xC0, new InstructionBuilder(0xC0, "RET NZ", cpuContext)
                .With(b => b
                    .WriteFlags                 (() => registers.Flags)
                    .IfFlagsUnset               (ProcessorFlags.Zero,
                        t => t
                            // PC = (SP)
                            .Input              (vars.Get<ushort>("COUNTER16"), () => memoryController.ReadDouble(registers.SP), 2)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => registers.SetPC(val))
                            // SP += 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                            .Cycle              ()
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val)))
                    .Cycle                      (2))
                .Compile());
            
            // RET Z
            this.opcodes.Add(0xC8, new InstructionBuilder(0xC8, "RET Z", cpuContext)
                .With(b => b
                    .WriteFlags                 (() => registers.Flags)
                    .IfFlagsSet                 (ProcessorFlags.Zero,
                        t => t
                            // PC = (SP)
                            .Input              (vars.Get<ushort>("COUNTER16"), () => memoryController.ReadDouble(registers.SP), 2)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => registers.SetPC(val))
                            // SP += 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                            .Cycle              ()
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val)))
                    .Cycle                      (2))
                .Compile());

            // RET NC
            this.opcodes.Add(0xD0, new InstructionBuilder(0xD0, "RET NC", cpuContext)
                .With(b => b
                    .WriteFlags                 (() => registers.Flags)
                    .IfFlagsUnset               (ProcessorFlags.Carry,
                        t => t
                            // PC = (SP)
                            .Input              (vars.Get<ushort>("COUNTER16"), () => memoryController.ReadDouble(registers.SP), 2)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => registers.SetPC(val))
                            // SP += 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                            .Cycle              ()
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val)))
                    .Cycle                      (2))
                .Compile());
            
            // RET C
            this.opcodes.Add(0xD8, new InstructionBuilder(0xD8, "RET C", cpuContext)
                .With(b => b
                    .WriteFlags                 (() => registers.Flags)
                    .IfFlagsSet                 (ProcessorFlags.Carry,
                        t => t
                            // PC = (SP)
                            .Input              (vars.Get<ushort>("COUNTER16"), () => memoryController.ReadDouble(registers.SP), 2)
                            .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => registers.SetPC(val))
                            // SP += 2
                            .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                            .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                            .Cycle              ()
                            .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val)))
                    .Cycle                      (2))
                .Compile());

            var rstMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0xC7, "RST", null, null, "00H", () => 0x00),
                new Mapping<ushort>(0xD7, "RST", null, null, "10H", () => 0x10),
                new Mapping<ushort>(0xE7, "RST", null, null, "20H", () => 0x20),
                new Mapping<ushort>(0xF7, "RST", null, null, "30H", () => 0x30),

                new Mapping<ushort>(0xCF, "RST", null, null, "08H", () => 0x08),
                new Mapping<ushort>(0xDF, "RST", null, null, "18H", () => 0x18),
                new Mapping<ushort>(0xEF, "RST", null, null, "28H", () => 0x28),
                new Mapping<ushort>(0xFF, "RST", null, null, "38H", () => 0x38),
            };

            foreach (var mapping in rstMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("PTR16"), mapping.Input)
                        // SP -= 2
                        .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                        .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                        .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                        .Cycle              ()
                        // (SP) = PC
                        .Input              (vars.Get<ushort>("COUNTER16"), () => registers.PC)
                        .Output             (vars.Get<ushort>("COUNTER16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                        // PC = a16
                        .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetPC(val))
                        .Cycle              ())
                .Compile());
            }

            #endregion

            #region 8-bit loads

            // TODO: maybe use alternative mnemonics?

            var load8Mappings = new Mapping<byte>[] // this is gonna hurt
            {
                new Mapping<byte>(0x02, "LD", "(BC)", val => memoryController.Write(registers.BC, val), "A", () => registers.A, outputCycles: 1),
                new Mapping<byte>(0x12, "LD", "(DE)", val => memoryController.Write(registers.DE, val), "A", () => registers.A, outputCycles: 1),
                new Mapping<byte>(0x0A, "LD", "A", val => registers.SetA(val), "(BC)", () => memoryController.Read(registers.BC), inputCycles: 1),
                new Mapping<byte>(0x1A, "LD", "A", val => registers.SetA(val), "(DE)", () => memoryController.Read(registers.DE), inputCycles: 1),
                
                new Mapping<byte>(0x06, "LD", "B", val => registers.SetB(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x16, "LD", "D", val => registers.SetD(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x26, "LD", "H", val => registers.SetH(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x36, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "d8", () => assembly.Read(), inputCycles: 1, outputCycles: 1),

                new Mapping<byte>(0x0E, "LD", "C", val => registers.SetC(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x1E, "LD", "E", val => registers.SetE(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x2E, "LD", "L", val => registers.SetL(val), "d8", () => assembly.Read(), inputCycles: 1),
                new Mapping<byte>(0x3E, "LD", "A", val => registers.SetA(val), "d8", () => assembly.Read(), inputCycles: 1),

                new Mapping<byte>(0x40, "LD", "B", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0x41, "LD", "B", val => registers.SetB(val), "C", () => registers.C),
                new Mapping<byte>(0x42, "LD", "B", val => registers.SetB(val), "D", () => registers.D),
                new Mapping<byte>(0x43, "LD", "B", val => registers.SetB(val), "E", () => registers.E),
                new Mapping<byte>(0x44, "LD", "B", val => registers.SetB(val), "H", () => registers.H),
                new Mapping<byte>(0x45, "LD", "B", val => registers.SetB(val), "L", () => registers.L),
                new Mapping<byte>(0x46, "LD", "B", val => registers.SetB(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x47, "LD", "B", val => registers.SetB(val), "A", () => registers.A),

                new Mapping<byte>(0x48, "LD", "C", val => registers.SetC(val), "B", () => registers.B),
                new Mapping<byte>(0x49, "LD", "C", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0x4A, "LD", "C", val => registers.SetC(val), "D", () => registers.D),
                new Mapping<byte>(0x4B, "LD", "C", val => registers.SetC(val), "E", () => registers.E),
                new Mapping<byte>(0x4C, "LD", "C", val => registers.SetC(val), "H", () => registers.H),
                new Mapping<byte>(0x4D, "LD", "C", val => registers.SetC(val), "L", () => registers.L),
                new Mapping<byte>(0x4E, "LD", "C", val => registers.SetC(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x4F, "LD", "C", val => registers.SetC(val), "A", () => registers.A),

                new Mapping<byte>(0x50, "LD", "D", val => registers.SetD(val), "B", () => registers.B),
                new Mapping<byte>(0x51, "LD", "D", val => registers.SetD(val), "C", () => registers.C),
                new Mapping<byte>(0x52, "LD", "D", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0x53, "LD", "D", val => registers.SetD(val), "E", () => registers.E),
                new Mapping<byte>(0x54, "LD", "D", val => registers.SetD(val), "H", () => registers.H),
                new Mapping<byte>(0x55, "LD", "D", val => registers.SetD(val), "L", () => registers.L),
                new Mapping<byte>(0x56, "LD", "D", val => registers.SetD(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x57, "LD", "D", val => registers.SetD(val), "A", () => registers.A),

                new Mapping<byte>(0x58, "LD", "E", val => registers.SetE(val), "B", () => registers.B),
                new Mapping<byte>(0x59, "LD", "E", val => registers.SetE(val), "C", () => registers.C),
                new Mapping<byte>(0x5A, "LD", "E", val => registers.SetE(val), "D", () => registers.D),
                new Mapping<byte>(0x5B, "LD", "E", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0x5C, "LD", "E", val => registers.SetE(val), "H", () => registers.H),
                new Mapping<byte>(0x5D, "LD", "E", val => registers.SetE(val), "L", () => registers.L),
                new Mapping<byte>(0x5E, "LD", "E", val => registers.SetE(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x5F, "LD", "E", val => registers.SetE(val), "A", () => registers.A),

                new Mapping<byte>(0x60, "LD", "H", val => registers.SetH(val), "B", () => registers.B),
                new Mapping<byte>(0x61, "LD", "H", val => registers.SetH(val), "C", () => registers.C),
                new Mapping<byte>(0x62, "LD", "H", val => registers.SetH(val), "D", () => registers.D),
                new Mapping<byte>(0x63, "LD", "H", val => registers.SetH(val), "E", () => registers.E),
                new Mapping<byte>(0x64, "LD", "H", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0x65, "LD", "H", val => registers.SetH(val), "L", () => registers.L),
                new Mapping<byte>(0x66, "LD", "H", val => registers.SetH(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x67, "LD", "H", val => registers.SetH(val), "A", () => registers.A),

                new Mapping<byte>(0x68, "LD", "L", val => registers.SetL(val), "B", () => registers.B),
                new Mapping<byte>(0x69, "LD", "L", val => registers.SetL(val), "C", () => registers.C),
                new Mapping<byte>(0x6A, "LD", "L", val => registers.SetL(val), "D", () => registers.D),
                new Mapping<byte>(0x6B, "LD", "L", val => registers.SetL(val), "E", () => registers.E),
                new Mapping<byte>(0x6C, "LD", "L", val => registers.SetL(val), "H", () => registers.H),
                new Mapping<byte>(0x6D, "LD", "L", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0x6E, "LD", "L", val => registers.SetL(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x6F, "LD", "L", val => registers.SetL(val), "A", () => registers.A),

                new Mapping<byte>(0x70, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "B", () => registers.B, outputCycles: 1),
                new Mapping<byte>(0x71, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "C", () => registers.C, outputCycles: 1),
                new Mapping<byte>(0x72, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "D", () => registers.D, outputCycles: 1),
                new Mapping<byte>(0x73, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "E", () => registers.E, outputCycles: 1),
                new Mapping<byte>(0x74, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "H", () => registers.H, outputCycles: 1),
                new Mapping<byte>(0x75, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "L", () => registers.L, outputCycles: 1),
                new Mapping<byte>(0x77, "LD", "(HL)", val => memoryController.Write(registers.HL, val), "A", () => registers.A, outputCycles: 1),

                new Mapping<byte>(0x78, "LD", "A", val => registers.SetA(val), "B", () => registers.B),
                new Mapping<byte>(0x79, "LD", "A", val => registers.SetA(val), "C", () => registers.C),
                new Mapping<byte>(0x7A, "LD", "A", val => registers.SetA(val), "D", () => registers.D),
                new Mapping<byte>(0x7B, "LD", "A", val => registers.SetA(val), "E", () => registers.E),
                new Mapping<byte>(0x7C, "LD", "A", val => registers.SetA(val), "H", () => registers.H),
                new Mapping<byte>(0x7D, "LD", "A", val => registers.SetA(val), "L", () => registers.L),
                new Mapping<byte>(0x7E, "LD", "A", val => registers.SetA(val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x7F, "LD", "A", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xE0, "LDH", "(a8)", val => memoryController.Write(0xFF00 + assembly.Read(), val), "A", () => registers.A, inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xF0, "LDH", "A", val => registers.SetA(val), "(a8)", () => memoryController.Read(0xFF00 + assembly.Read()), inputCycles: 2),

                new Mapping<byte>(0xE2, "LD", "(C)", val => memoryController.Write(0xFF00 + registers.C, val), "A", () => registers.A, outputCycles: 1),
                new Mapping<byte>(0xF2, "LD", "A", val => registers.SetA(val), "(a8)", () => memoryController.Read(0xFF00 + registers.C), inputCycles: 1),

                new Mapping<byte>(0xEA, "LD", "(a16)", val => memoryController.Write(assembly.ReadDouble(), val), "A", () => registers.A, inputCycles: 2, outputCycles: 1),
                new Mapping<byte>(0xFA, "LD", "A", val => registers.SetA(val), "(a16)", () => memoryController.Read(assembly.ReadDouble()), inputCycles: 3),
            };

            foreach (var mapping in load8Mappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input  (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .Output (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle  ())
                    .Compile());
            }

            // LD (HL+), A
            this.opcodes.Add(0x22, new InstructionBuilder(0x22, "LD (HL+), A", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                        .Output             (vars.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val), 1)
                        .Input              (vars.Get<ushort>("HL"), () => registers.HL)
                        .Add<ushort, ushort>(vars.Get<ushort>("HL"), 1)
                        .Output             (vars.Get<ushort>("HL"), (ushort hl) => registers.SetHL(hl))
                        .Cycle              ())
                    .Compile());
            
            // LD (HL-), A
            this.opcodes.Add(0x32, new InstructionBuilder(0x32, "LD (HL-), A", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                        .Output             (vars.Get<byte>("VAL8"), (byte val) => memoryController.Write(registers.HL, val), 1)
                        .Input              (vars.Get<ushort>("HL"), () => registers.HL)
                        .Subtract<ushort>   (vars.Get<ushort>("HL"), 1)
                        .Output             (vars.Get<ushort>("HL"), (ushort hl) => registers.SetHL(hl))
                        .Cycle              ())
                    .Compile());
            
            // LD A, (HL+)
            this.opcodes.Add(0x2A, new InstructionBuilder(0x2A, "LD A, (HL+)", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), () => memoryController.Read(registers.HL), 1)
                        .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                        .Input              (vars.Get<ushort>("HL"), () => registers.HL)
                        .Add<ushort, ushort>(vars.Get<ushort>("HL"), 1)
                        .Output             (vars.Get<ushort>("HL"), (ushort hl) => registers.SetHL(hl))
                        .Cycle              ())
                    .Compile());
            
            // LD A, (HL-)
            this.opcodes.Add(0x3A, new InstructionBuilder(0x3A, "LD A, (HL-)", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), () => memoryController.Read(registers.HL), 1)
                        .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                        .Input              (vars.Get<ushort>("HL"), () => registers.HL)
                        .Subtract<ushort>   (vars.Get<ushort>("HL"), 1)
                        .Output             (vars.Get<ushort>("HL"), (ushort hl) => registers.SetHL(hl))
                        .Cycle              ())
                    .Compile());

            #endregion

            #region 16-bit transfers

            var load16Mappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0x01, "LD", "BC", val => registers.SetBC(val), "d16", () => assembly.ReadDouble(), inputCycles: 2),
                new Mapping<ushort>(0x11, "LD", "DE", val => registers.SetDE(val), "d16", () => assembly.ReadDouble(), inputCycles: 2),
                new Mapping<ushort>(0x21, "LD", "HL", val => registers.SetHL(val), "d16", () => assembly.ReadDouble(), inputCycles: 2),
                new Mapping<ushort>(0x31, "LD", "SP", val => registers.SetSP(val), "d16", () => assembly.ReadDouble(), inputCycles: 2),

                new Mapping<ushort>(0x08, "LD", "(a16)", val => memoryController.WriteDouble(assembly.ReadDouble(), val), "SP", () => registers.SP, inputCycles: 2, outputCycles: 2),

                new Mapping<ushort>(0xF9, "LD", "SP", val => registers.SetSP(val), "HL", () => registers.HL, inputCycles: 1),
            };

            foreach (var mapping in load16Mappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input  (vars.Get<ushort>("VAL16"), mapping.Input, mapping.InputCycles)
                        .Output (vars.Get<ushort>("VAL16"), mapping.Output, mapping.OutputCycles)
                        .Cycle  ())
                    .Compile());
            }

            // LD HL, SP + r8
            this.opcodes.Add(0xF8, new InstructionBuilder(0xF8, "LD HL, SP + r8", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("VAL16"), () => registers.SP)
                        .Add<ushort, sbyte> (vars.Get<ushort>("VAL16"), () => assembly.Read().TwosComplement())
                        .Cycle              (2)
                        .UnsetFlags         (ProcessorFlags.Zero | ProcessorFlags.Arithmetic)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<ushort>("VAL16"), (ushort val) => registers.SetHL(val))
                        .Cycle              ())
                    .Compile());

            var pushMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0xC5, "PUSH", null, null, "BC", () => registers.BC),
                new Mapping<ushort>(0xD5, "PUSH", null, null, "DE", () => registers.DE),
                new Mapping<ushort>(0xE5, "PUSH", null, null, "HL", () => registers.HL),
                new Mapping<ushort>(0xF5, "PUSH", null, null, "AF", () => registers.AF),
            };

            foreach (var mapping in pushMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                        .Subtract<ushort>   (vars.Get<ushort>("STACK16"), 2)
                        .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                        .Cycle              ()
                        .Input              (vars.Get<ushort>("PTR16"), mapping.Input)
                        .Output             (vars.Get<ushort>("PTR16"), (ushort val) => memoryController.WriteDouble(registers.SP, val), 2)
                        .Cycle              ())
                    .Compile());
            }

            var popMappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0xC1, "POP", "BC", val => registers.SetBC(val)),
                new Mapping<ushort>(0xD1, "POP", "DE", val => registers.SetDE(val)),
                new Mapping<ushort>(0xE1, "POP", "HL", val => registers.SetHL(val)),
            };

            foreach (var mapping in popMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("PTR16"), () => memoryController.ReadDouble(registers.SP), 2)
                        .Output             (vars.Get<ushort>("PTR16"), mapping.Output)
                        .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                        .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                        .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                        .Cycle              ())
                    .Compile());
            }

            // POP AF
            this.opcodes.Add(0xF1, new InstructionBuilder(0xF1, "POP AF", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("PTR16"), () => memoryController.ReadDouble(registers.SP), 2)
                        .Output             (vars.Get<ushort>("PTR16"), (ushort val) => registers.SetAF((ushort)(val & 0xFFF0)))
                        .Input              (vars.Get<ushort>("STACK16"), () => registers.SP)
                        .Add<ushort, ushort>(vars.Get<ushort>("STACK16"), 2)
                        .Output             (vars.Get<ushort>("STACK16"), (ushort val) => registers.SetSP(val))
                        .Cycle              ())
                    .Compile());

            #endregion

            #region 8-bit arithmetic

            var incMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x04, "INC", "B", val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0x0C, "INC", "C", val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0x14, "INC", "D", val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0x1C, "INC", "E", val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0x24, "INC", "H", val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0x2C, "INC", "L", val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0x34, "INC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0x3C, "INC", "A", val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in incMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .Add<byte, byte>(vars.Get<byte>("VAL8"), 1)
                        .ReadFlags      (flags => registers.SetFlags(flags, ProcessorFlags.Zero | ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry))
                        .Output         (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle          ())
                    .Compile());
            }

            var decMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x05, "DEC", "B", val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0x0D, "DEC", "C", val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0x15, "DEC", "D", val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0x1D, "DEC", "E", val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0x25, "DEC", "H", val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0x2D, "DEC", "L", val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0x35, "DEC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0x3D, "DEC", "A", val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in decMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .Subtract<byte> (vars.Get<byte>("VAL8"), 1)
                        .ReadFlags      (flags => registers.SetFlags(flags, ProcessorFlags.Zero | ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry))
                        .Output         (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle          ())
                    .Compile());
            }

            var addMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x80, "ADD", "A", null, "B", () => registers.B),
                new Mapping<byte>(0x81, "ADD", "A", null, "C", () => registers.C),
                new Mapping<byte>(0x82, "ADD", "A", null, "D", () => registers.D),
                new Mapping<byte>(0x83, "ADD", "A", null, "E", () => registers.E),
                new Mapping<byte>(0x84, "ADD", "A", null, "H", () => registers.H),
                new Mapping<byte>(0x85, "ADD", "A", null, "L", () => registers.L),
                new Mapping<byte>(0x86, "ADD", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x87, "ADD", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xC6, "ADD", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in addMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle          (mapping.InputCycles)
                        .ReadFlags      (flags => registers.SetFlags(flags))
                        .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle          ())
                    .Compile());
            }

            var addCarryMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x88, "ADC", "A", null, "B", () => registers.B),
                new Mapping<byte>(0x89, "ADC", "A", null, "C", () => registers.C),
                new Mapping<byte>(0x8A, "ADC", "A", null, "D", () => registers.D),
                new Mapping<byte>(0x8B, "ADC", "A", null, "E", () => registers.E),
                new Mapping<byte>(0x8C, "ADC", "A", null, "H", () => registers.H),
                new Mapping<byte>(0x8D, "ADC", "A", null, "L", () => registers.L),
                new Mapping<byte>(0x8E, "ADC", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x8F, "ADC", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xCE, "ADC", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in addCarryMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .WriteFlags     (() => registers.Flags)
                        .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input, true)
                        .Cycle          (mapping.InputCycles)
                        .ReadFlags      (flags => registers.SetFlags(flags))
                        .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle          ())
                    .Compile());
            }

            var subMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x90, "SUB", "A", null, "B", () => registers.B),
                new Mapping<byte>(0x91, "SUB", "A", null, "C", () => registers.C),
                new Mapping<byte>(0x92, "SUB", "A", null, "D", () => registers.D),
                new Mapping<byte>(0x93, "SUB", "A", null, "E", () => registers.E),
                new Mapping<byte>(0x94, "SUB", "A", null, "H", () => registers.H),
                new Mapping<byte>(0x95, "SUB", "A", null, "L", () => registers.L),
                new Mapping<byte>(0x96, "SUB", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x97, "SUB", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xD6, "SUB", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in subMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .Subtract<byte> (vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle          (mapping.InputCycles)
                        .ReadFlags      (flags => registers.SetFlags(flags))
                        .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle          ())
                    .Compile());
            }

            var subCarryMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0x98, "SBC", "A", null, "B", () => registers.B),
                new Mapping<byte>(0x99, "SBC", "A", null, "C", () => registers.C),
                new Mapping<byte>(0x9A, "SBC", "A", null, "D", () => registers.D),
                new Mapping<byte>(0x9B, "SBC", "A", null, "E", () => registers.E),
                new Mapping<byte>(0x9C, "SBC", "A", null, "H", () => registers.H),
                new Mapping<byte>(0x9D, "SBC", "A", null, "L", () => registers.L),
                new Mapping<byte>(0x9E, "SBC", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0x9F, "SBC", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xDE, "SBC", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in subCarryMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .WriteFlags     (() => registers.Flags)
                        .Add<byte, byte>(vars.Get<byte>("VAL8"), mapping.Input, true)
                        .Cycle          (mapping.InputCycles)
                        .ReadFlags      (flags => registers.SetFlags(flags))
                        .Output         (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle          ())
                    .Compile());
            }

            var andMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xA0, "AND", "A", null, "B", () => registers.B),
                new Mapping<byte>(0xA1, "AND", "A", null, "C", () => registers.C),
                new Mapping<byte>(0xA2, "AND", "A", null, "D", () => registers.D),
                new Mapping<byte>(0xA3, "AND", "A", null, "E", () => registers.E),
                new Mapping<byte>(0xA4, "AND", "A", null, "H", () => registers.H),
                new Mapping<byte>(0xA5, "AND", "A", null, "L", () => registers.L),
                new Mapping<byte>(0xA6, "AND", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0xA7, "AND", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xE6, "AND", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in andMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .And<byte>  (vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle      (mapping.InputCycles)
                        .ReadFlags  (flags => registers.SetFlags(flags))
                        .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle      ())
                    .Compile());
            }

            var xorMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xA8, "XOR", "A", null, "B", () => registers.B),
                new Mapping<byte>(0xA9, "XOR", "A", null, "C", () => registers.C),
                new Mapping<byte>(0xAA, "XOR", "A", null, "D", () => registers.D),
                new Mapping<byte>(0xAB, "XOR", "A", null, "E", () => registers.E),
                new Mapping<byte>(0xAC, "XOR", "A", null, "H", () => registers.H),
                new Mapping<byte>(0xAD, "XOR", "A", null, "L", () => registers.L),
                new Mapping<byte>(0xAE, "XOR", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0xAF, "XOR", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xEE, "XOR", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in xorMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .Xor<byte>  (vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle      (mapping.InputCycles)
                        .ReadFlags  (flags => registers.SetFlags(flags))
                        .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle      ())
                    .Compile());
            }

            var orMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xB0, "OR", "A", null, "B", () => registers.B),
                new Mapping<byte>(0xB1, "OR", "A", null, "C", () => registers.C),
                new Mapping<byte>(0xB2, "OR", "A", null, "D", () => registers.D),
                new Mapping<byte>(0xB3, "OR", "A", null, "E", () => registers.E),
                new Mapping<byte>(0xB4, "OR", "A", null, "H", () => registers.H),
                new Mapping<byte>(0xB5, "OR", "A", null, "L", () => registers.L),
                new Mapping<byte>(0xB6, "OR", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0xB7, "OR", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xF6, "OR", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in orMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input      (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .Or<byte>   (vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle      (mapping.InputCycles)
                        .ReadFlags  (flags => registers.SetFlags(flags))
                        .Output     (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                        .Cycle      ())
                    .Compile());
            }

            var cpMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xB8, "CP", "A", null, "B", () => registers.B),
                new Mapping<byte>(0xB9, "CP", "A", null, "C", () => registers.C),
                new Mapping<byte>(0xBA, "CP", "A", null, "D", () => registers.D),
                new Mapping<byte>(0xBB, "CP", "A", null, "E", () => registers.E),
                new Mapping<byte>(0xBC, "CP", "A", null, "H", () => registers.H),
                new Mapping<byte>(0xBD, "CP", "A", null, "L", () => registers.L),
                new Mapping<byte>(0xBE, "CP", "A", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1),
                new Mapping<byte>(0xBF, "CP", "A", null, "A", () => registers.A),

                new Mapping<byte>(0xFE, "CP", "A", null, "d8", () => assembly.Read(), inputCycles: 1),
            };

            foreach (var mapping in cpMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                        .Subtract<byte> (vars.Get<byte>("VAL8"), mapping.Input)
                        .Cycle          (mapping.InputCycles)
                        .ReadFlags      (flags => registers.SetFlags(flags))
                        .Cycle          ())
                    .Compile());
            }

            // DAA
            this.opcodes.Add(0x27, new InstructionBuilder(0x27, "DAA", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .WriteFlags         (() => registers.Flags)
                    .DecimalAdjust<byte>(vars.Get<byte>("VAL8"))
                    .ReadFlags          (flags => registers.SetFlags(flags, ProcessorFlags.Zero | ProcessorFlags.HalfCarry | ProcessorFlags.Carry))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Cycle              ())
                .Compile());
            
            // CPL
            this.opcodes.Add(0x2F, new InstructionBuilder(0x2F, "CPL", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.Accumulator)
                    .WriteFlags         (() => registers.Flags)
                    .Complement<byte>   (vars.Get<byte>("VAL8"))
                    .ReadFlags          (flags => registers.SetFlags(flags, ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetAccumulator(val))
                    .Cycle              ())
                .Compile());
            
            // SCF
            this.opcodes.Add(0x37, new InstructionBuilder(0x37, "SCF", cpuContext)
                .With(b => b
                    .WriteFlags         (() => registers.Flags)
                    .SetFlags           (ProcessorFlags.Carry)
                    .ReadFlags          (flags => registers.SetFlags(flags, ProcessorFlags.Carry))
                    .Cycle              ())
                .Compile());
            
            // CCF
            // this could've been done better, but why waste a good opportunity to test out conditional expressions?
            this.opcodes.Add(0x3F, new InstructionBuilder(0x3F, "CCF", cpuContext)
                .With(b => b
                    .WriteFlags         (() => registers.Flags)
                    .IfFlagsSet         (ProcessorFlags.Carry,
                        t => t
                            .UnsetFlags (ProcessorFlags.Carry),
                        f => f
                            .SetFlags   (ProcessorFlags.Carry))
                    .UnsetFlags         (ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry)
                    .ReadFlags          (flags => registers.SetFlags(flags, ProcessorFlags.Arithmetic | ProcessorFlags.HalfCarry | ProcessorFlags.Carry))
                    .Cycle              ())
                .Compile());

            #endregion

            #region 16-bit airthmetic

            var inc16Mappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0x03, "INC", "BC", val => registers.SetBC(val), null, () => registers.BC),
                new Mapping<ushort>(0x13, "INC", "DE", val => registers.SetDE(val), null, () => registers.DE),
                new Mapping<ushort>(0x23, "INC", "HL", val => registers.SetHL(val), null, () => registers.HL),
                new Mapping<ushort>(0x33, "INC", "SP", val => registers.SetSP(val), null, () => registers.SP),
            };

            foreach (var mapping in inc16Mappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("VAL16"), mapping.Input)
                        .Add<ushort, ushort>(vars.Get<ushort>("VAL16"), 1)
                        .Output             (vars.Get<ushort>("VAL16"), mapping.Output)
                        .Cycle              (2))
                    .Compile());
            }

            var dec16Mappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0x0B, "DEC", "BC", val => registers.SetBC(val), null, () => registers.BC),
                new Mapping<ushort>(0x1B, "DEC", "DE", val => registers.SetDE(val), null, () => registers.DE),
                new Mapping<ushort>(0x2B, "DEC", "HL", val => registers.SetHL(val), null, () => registers.HL),
                new Mapping<ushort>(0x3B, "DEC", "SP", val => registers.SetSP(val), null, () => registers.SP),
            };

            foreach (var mapping in dec16Mappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("VAL16"), mapping.Input)
                        .Subtract<ushort>   (vars.Get<ushort>("VAL16"), 1)
                        .Output             (vars.Get<ushort>("VAL16"), mapping.Output)
                        .Cycle              (2))
                    .Compile());
            }

            var add16Mappings = new Mapping<ushort>[]
            {
                new Mapping<ushort>(0x09, "ADD", "HL", null, "BC", () => registers.BC),
                new Mapping<ushort>(0x19, "ADD", "HL", null, "DE", () => registers.DE),
                new Mapping<ushort>(0x29, "ADD", "HL", null, "HL", () => registers.HL),
                new Mapping<ushort>(0x39, "ADD", "HL", null, "SP", () => registers.SP),
            };

            foreach (var mapping in add16Mappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("VAL16"), () => registers.HL)
                        .WriteFlags         (() => registers.Flags)
                        .Add<ushort, ushort>(vars.Get<ushort>("VAL16"), mapping.Input)
                        .ReadFlags          (flags => registers.SetFlags(flags, ProcessorFlags.All ^ ProcessorFlags.Zero))
                        .Output             (vars.Get<ushort>("VAL16"), (ushort val) => registers.SetHL(val))
                        .Cycle              (2))
                    .Compile());
            }

            // ADD SP, r8
            this.opcodes.Add(0xE8, new InstructionBuilder(0xE8, "ADD SP, r8", cpuContext)
                    .With(b => b
                        .Input              (vars.Get<ushort>("VAL16"), () => registers.SP)
                        .Add<ushort, sbyte> (vars.Get<ushort>("VAL16"), () => assembly.Read().TwosComplement())
                        .Cycle              (2)
                        .UnsetFlags         (ProcessorFlags.Zero | ProcessorFlags.Arithmetic)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<ushort>("VAL16"), (ushort val) => registers.SetSP(val))
                        .Cycle              (2))
                    .Compile());

            #endregion

            #region Shifts

            // RLCA
            this.opcodes.Add(0x07, new InstructionBuilder(0x07, "RLCA", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                    .RotateLeft<byte>   (vars.Get<byte>("VAL8"), true)
                    .UnsetFlags         (ProcessorFlags.Zero)
                    .ReadFlags          (flags => registers.SetFlags(flags))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Cycle              ())
                .Compile());

            var rlcMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_00, "RLC", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_01, "RLC", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_02, "RLC", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_03, "RLC", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_04, "RLC", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_05, "RLC", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_06, "RLC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_07, "RLC", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in rlcMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .RotateLeft<byte>   (vars.Get<byte>("VAL8"), true)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            // RRCA
            this.opcodes.Add(0x0F, new InstructionBuilder(0x0F, "RRCA", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                    .RotateRight<byte>  (vars.Get<byte>("VAL8"), true)
                    .UnsetFlags         (ProcessorFlags.Zero)
                    .ReadFlags          (flags => registers.SetFlags(flags))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Cycle              ())
                .Compile());

            var rrcMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_08, "RRC", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_09, "RRC", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_0A, "RRC", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_0B, "RRC", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_0C, "RRC", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_0D, "RRC", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_0E, "RRC", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_0F, "RRC", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in rrcMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .RotateRight<byte>  (vars.Get<byte>("VAL8"), true)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            // RLA
            this.opcodes.Add(0x17, new InstructionBuilder(0x17, "RLA", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                    .WriteFlags         (() => registers.Flags)
                    .RotateLeft<byte>   (vars.Get<byte>("VAL8"), false)
                    .UnsetFlags         (ProcessorFlags.Zero)
                    .ReadFlags          (flags => registers.SetFlags(flags))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Cycle              ())
                .Compile());

            var rlMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_10, "RL", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_11, "RL", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_12, "RL", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_13, "RL", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_14, "RL", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_15, "RL", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_16, "RL", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_17, "RL", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in rlMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .WriteFlags         (() => registers.Flags)
                        .RotateLeft<byte>   (vars.Get<byte>("VAL8"), false)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            // RRA
            this.opcodes.Add(0x1F, new InstructionBuilder(0x1F, "RRCA", cpuContext)
                .With(b => b
                    .Input              (vars.Get<byte>("VAL8"), () => registers.A)
                    .WriteFlags         (() => registers.Flags)
                    .RotateRight<byte>  (vars.Get<byte>("VAL8"), false)
                    .UnsetFlags         (ProcessorFlags.Zero)
                    .ReadFlags          (flags => registers.SetFlags(flags))
                    .Output             (vars.Get<byte>("VAL8"), (byte val) => registers.SetA(val))
                    .Cycle              ())
                .Compile());

            var rrMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_18, "RR", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_19, "RR", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_1A, "RR", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_1B, "RR", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_1C, "RR", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_1D, "RR", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_1E, "RR", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_1F, "RR", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in rrMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .WriteFlags         (() => registers.Flags)
                        .RotateRight<byte>  (vars.Get<byte>("VAL8"), false)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            var slaMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_20, "SLA", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_21, "SLA", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_22, "SLA", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_23, "SLA", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_24, "SLA", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_25, "SLA", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_26, "SLA", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_27, "SLA", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in slaMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .ShiftLeft<byte>    (vars.Get<byte>("VAL8"), false)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            var sraMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_28, "SRA", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_29, "SRA", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_2A, "SRA", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_2B, "SRA", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_2C, "SRA", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_2D, "SRA", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_2E, "SRA", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_2F, "SRA", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in sraMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .ShiftRight<byte>   (vars.Get<byte>("VAL8"), true)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            var swapMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_30, "SWAP", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_31, "SWAP", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_32, "SWAP", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_33, "SWAP", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_34, "SWAP", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_35, "SWAP", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_36, "SWAP", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_37, "SWAP", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in swapMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input      (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .Swap<byte> (vars.Get<byte>("VAL8"))
                        .ReadFlags  (flags => registers.SetFlags(flags))
                        .Output     (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle      (2))
                    .Compile());
            }

            var srlMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_38, "SRL", "B",  val => registers.SetB(val), null, () => registers.B),
                new Mapping<byte>(0xCB_39, "SRL", "C",  val => registers.SetC(val), null, () => registers.C),
                new Mapping<byte>(0xCB_3A, "SRL", "D",  val => registers.SetD(val), null, () => registers.D),
                new Mapping<byte>(0xCB_3B, "SRL", "E",  val => registers.SetE(val), null, () => registers.E),
                new Mapping<byte>(0xCB_3C, "SRL", "H",  val => registers.SetH(val), null, () => registers.H),
                new Mapping<byte>(0xCB_3D, "SRL", "L",  val => registers.SetL(val), null, () => registers.L),
                new Mapping<byte>(0xCB_3E, "SRL", "(HL)", val => memoryController.Write(registers.HL, val), null, () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_3F, "SRL", "A",  val => registers.SetA(val), null, () => registers.A),
            };

            foreach (var mapping in srlMappings)
            {
                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input              (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .ShiftRight<byte>   (vars.Get<byte>("VAL8"), false)
                        .ReadFlags          (flags => registers.SetFlags(flags))
                        .Output             (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle              (2))
                    .Compile());
            }

            #endregion

            #region Bit operations

            var testMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_40, "BIT", "0", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_41, "BIT", "0", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_42, "BIT", "0", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_43, "BIT", "0", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_44, "BIT", "0", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_45, "BIT", "0", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_46, "BIT", "0", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_47, "BIT", "0", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_48, "BIT", "1", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_49, "BIT", "1", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_4A, "BIT", "1", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_4B, "BIT", "1", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_4C, "BIT", "1", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_4D, "BIT", "1", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_4E, "BIT", "1", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_4F, "BIT", "1", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_50, "BIT", "2", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_51, "BIT", "2", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_52, "BIT", "2", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_53, "BIT", "2", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_54, "BIT", "2", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_55, "BIT", "2", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_56, "BIT", "2", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_57, "BIT", "2", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_58, "BIT", "3", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_59, "BIT", "3", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_5A, "BIT", "3", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_5B, "BIT", "3", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_5C, "BIT", "3", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_5D, "BIT", "3", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_5E, "BIT", "3", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_5F, "BIT", "3", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_60, "BIT", "4", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_61, "BIT", "4", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_62, "BIT", "4", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_63, "BIT", "4", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_64, "BIT", "4", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_65, "BIT", "4", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_66, "BIT", "4", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_67, "BIT", "4", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_68, "BIT", "5", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_69, "BIT", "5", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_6A, "BIT", "5", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_6B, "BIT", "5", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_6C, "BIT", "5", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_6D, "BIT", "5", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_6E, "BIT", "5", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_6F, "BIT", "5", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_70, "BIT", "6", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_71, "BIT", "6", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_72, "BIT", "6", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_73, "BIT", "6", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_74, "BIT", "6", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_75, "BIT", "6", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_76, "BIT", "6", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_77, "BIT", "6", null, "A", () => registers.A),

                new Mapping<byte>(0xCB_78, "BIT", "7", null, "B", () => registers.B),
                new Mapping<byte>(0xCB_79, "BIT", "7", null, "C", () => registers.C),
                new Mapping<byte>(0xCB_7A, "BIT", "7", null, "D", () => registers.D),
                new Mapping<byte>(0xCB_7B, "BIT", "7", null, "E", () => registers.E),
                new Mapping<byte>(0xCB_7C, "BIT", "7", null, "H", () => registers.H),
                new Mapping<byte>(0xCB_7D, "BIT", "7", null, "L", () => registers.L),
                new Mapping<byte>(0xCB_7E, "BIT", "7", null, "(HL)", () => memoryController.Read(registers.HL), inputCycles: 2),
                new Mapping<byte>(0xCB_7F, "BIT", "7", null, "A", () => registers.A),
            };

            foreach (var mapping in testMappings)
            {
                var bit = (mapping.Opcode - 0xCB_40) / 8;

                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .TestBit<byte>  (vars.Get<byte>("VAL8"), bit)
                        .ReadFlags      (flags => registers.SetFlags(flags, ProcessorFlags.All ^ ProcessorFlags.Carry))
                        .Cycle          (2))
                    .Compile());
            }

            var setBitMappings = new Mapping<byte>[]
            {
                new Mapping<byte>(0xCB_80, "RES", "0", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_81, "RES", "0", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_82, "RES", "0", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_83, "RES", "0", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_84, "RES", "0", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_85, "RES", "0", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_86, "RES", "0", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_87, "RES", "0", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_88, "RES", "1", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_89, "RES", "1", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_8A, "RES", "1", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_8B, "RES", "1", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_8C, "RES", "1", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_8D, "RES", "1", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_8E, "RES", "1", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_8F, "RES", "1", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_90, "RES", "2", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_91, "RES", "2", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_92, "RES", "2", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_93, "RES", "2", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_94, "RES", "2", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_95, "RES", "2", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_96, "RES", "2", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_97, "RES", "2", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_98, "RES", "3", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_99, "RES", "3", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_9A, "RES", "3", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_9B, "RES", "3", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_9C, "RES", "3", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_9D, "RES", "3", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_9E, "RES", "3", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_9F, "RES", "3", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_A0, "RES", "4", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_A1, "RES", "4", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_A2, "RES", "4", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_A3, "RES", "4", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_A4, "RES", "4", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_A5, "RES", "4", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_A6, "RES", "4", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_A7, "RES", "4", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_A8, "RES", "5", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_A9, "RES", "5", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_AA, "RES", "5", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_AB, "RES", "5", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_AC, "RES", "5", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_AD, "RES", "5", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_AE, "RES", "5", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_AF, "RES", "5", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_B0, "RES", "6", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_B1, "RES", "6", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_B2, "RES", "6", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_B3, "RES", "6", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_B4, "RES", "6", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_B5, "RES", "6", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_B6, "RES", "6", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_B7, "RES", "6", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_B8, "RES", "7", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_B9, "RES", "7", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_BA, "RES", "7", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_BB, "RES", "7", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_BC, "RES", "7", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_BD, "RES", "7", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_BE, "RES", "7", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_BF, "RES", "7", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_C0, "SET", "0", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_C1, "SET", "0", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_C2, "SET", "0", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_C3, "SET", "0", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_C4, "SET", "0", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_C5, "SET", "0", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_C6, "SET", "0", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_C7, "SET", "0", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_C8, "SET", "1", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_C9, "SET", "1", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_CA, "SET", "1", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_CB, "SET", "1", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_CC, "SET", "1", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_CD, "SET", "1", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_CE, "SET", "1", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_CF, "SET", "1", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_D0, "SET", "2", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_D1, "SET", "2", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_D2, "SET", "2", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_D3, "SET", "2", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_D4, "SET", "2", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_D5, "SET", "2", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_D6, "SET", "2", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_D7, "SET", "2", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_D8, "SET", "3", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_D9, "SET", "3", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_DA, "SET", "3", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_DB, "SET", "3", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_DC, "SET", "3", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_DD, "SET", "3", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_DE, "SET", "3", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_DF, "SET", "3", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_E0, "SET", "4", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_E1, "SET", "4", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_E2, "SET", "4", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_E3, "SET", "4", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_E4, "SET", "4", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_E5, "SET", "4", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_E6, "SET", "4", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_E7, "SET", "4", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_E8, "SET", "5", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_E9, "SET", "5", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_EA, "SET", "5", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_EB, "SET", "5", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_EC, "SET", "5", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_ED, "SET", "5", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_EE, "SET", "5", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_EF, "SET", "5", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_F0, "SET", "6", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_F1, "SET", "6", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_F2, "SET", "6", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_F3, "SET", "6", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_F4, "SET", "6", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_F5, "SET", "6", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_F6, "SET", "6", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_F7, "SET", "6", val => registers.SetA(val), "A", () => registers.A),

                new Mapping<byte>(0xCB_F8, "SET", "7", val => registers.SetB(val), "B", () => registers.B),
                new Mapping<byte>(0xCB_F9, "SET", "7", val => registers.SetC(val), "C", () => registers.C),
                new Mapping<byte>(0xCB_FA, "SET", "7", val => registers.SetD(val), "D", () => registers.D),
                new Mapping<byte>(0xCB_FB, "SET", "7", val => registers.SetE(val), "E", () => registers.E),
                new Mapping<byte>(0xCB_FC, "SET", "7", val => registers.SetH(val), "H", () => registers.H),
                new Mapping<byte>(0xCB_FD, "SET", "7", val => registers.SetL(val), "L", () => registers.L),
                new Mapping<byte>(0xCB_FE, "SET", "7", val => memoryController.Write(registers.HL, val), "(HL)", () => memoryController.Read(registers.HL), inputCycles: 1, outputCycles: 1),
                new Mapping<byte>(0xCB_FF, "SET", "7", val => registers.SetA(val), "A", () => registers.A),
            };

            foreach (var mapping in setBitMappings)
            {
                var bit = ((mapping.Opcode - 0xCB_80) / 8) % 8;
                var value = mapping.Opcode > 0xCB_BF;

                this.opcodes.Add(mapping.Opcode, new InstructionBuilder(mapping.Opcode, mapping.Mnemonic, cpuContext)
                    .With(b => b
                        .Input          (vars.Get<byte>("VAL8"), mapping.Input, mapping.InputCycles)
                        .SetBit<byte>   (vars.Get<byte>("VAL8"), bit, value)
                        .Output         (vars.Get<byte>("VAL8"), mapping.Output, mapping.OutputCycles)
                        .Cycle          (2))
                    .Compile());
            }

            #endregion
        }
    }
}