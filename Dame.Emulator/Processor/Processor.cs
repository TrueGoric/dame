using System;
using System.Collections.Generic;
using Dame.Emulator.Accessors;
using Dame.Emulator.Exceptions;
using Dame.Emulator.Instructions;
using Dame.Emulator.Memory;

namespace Dame.Emulator.Processor
{
    public sealed partial class Processor
    {
        private ProcessorExecutionContext cpuContext;
        private RegisterBank registers;
        private MemoryController memoryController;

        private AssemblyAccessor assembly;

        private Dictionary<int, Instruction> opcodes;

        public ProcessorExecutionContext Context => cpuContext;

        public Processor(RegisterBank registerBank, MemoryController memory)
        {
            registers = registerBank;
            memoryController = memory;
            cpuContext = new ProcessorExecutionContext(registerBank, memory);

            assembly = new AssemblyAccessor(memoryController, registers);

            opcodes = new Dictionary<int, Instruction>();

            InitRegisters();
        }

        public void Step()
        {
            int opcode;

            // Console.Write($"{registers.PC.ToString("X")} :: ");

            opcode = assembly.Read();

            if (registers.PC >= 0x21)
                opcode = opcode;

            if (opcode == 0xCB)
            {
                opcode <<= 8;
                opcode += assembly.Read();
            }

            ProcessInstruction(opcode);
        }

        private void ProcessInstruction(int opcode)
        {
            Instruction instruction;

            if (!opcodes.TryGetValue(opcode, out instruction))
                throw new InstructionNotImplementedException(opcode);

            // Console.WriteLine(instruction.Name);
            
            instruction.Invoker();
        }

        private void InitRegisters()
        {
            registers.AF = 0x01B0;
            registers.BC = 0x0013;
            registers.DE = 0x00D8;
            registers.HL = 0x014D;

            registers.SP = 0xFFFE;
        }
    }
}