using System;

namespace Dame.Emulator.Instructions
{
    public readonly struct Operand
    {
        public OperandType Type { get; }
        public int Parameter { get; }

        public Register Register
        {
            get => Type == OperandType.Register || Type == OperandType.RegisterLong
                ? (Register)Parameter
                : throw new InvalidOperationException();
        }

        public int Address => Parameter;

        public Operand(Register register)
        {
            Parameter = (int)register;

            switch (register)
            {
                case Register.A:
                case Register.F:
                case Register.B:
                case Register.C:
                case Register.D:
                case Register.E:
                case Register.H:
                case Register.L:
                    Type = OperandType.Register;
                    break;
                
                case Register.AF:
                case Register.BC:
                case Register.DE:
                case Register.HL:
                case Register.SP:
                case Register.PC:
                    Type = OperandType.RegisterLong;
                    break;

                default:
                    throw new ArgumentException(nameof(register));
            }
        }

        public Operand(int memoryAddress, bool isLong)
        {
            Type = isLong ? OperandType.MemoryLong : OperandType.Memory;
            Parameter = memoryAddress;
        }
    }
}
