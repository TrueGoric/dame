using System;
using System.IO;

namespace Dame.Emulator.Instructions
{
    public readonly struct Operand
    {
        public OperandType Type { get; }
        public int Parameter { get; }

        public Register Register => Type == OperandType.RegisterValue || Type == OperandType.RegisterValueLong
                ? (Register)Parameter
                : throw new InvalidOperationException();

        public int Address => Type == OperandType.Memory || Type == OperandType.MemoryLong
                ? Parameter
                : throw new InvalidOperationException();
        
        public Operand(Register register, bool isPointer)
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
                    Type = isPointer ? OperandType.RegisterAddress : OperandType.RegisterValue;
                    break;
                
                case Register.AF:
                case Register.BC:
                case Register.DE:
                case Register.HL:
                case Register.SP:
                case Register.PC:
                    Type = isPointer ? OperandType.RegisterAddressLong : OperandType.RegisterValueLong;
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

        public Operand(int constant)
        {
            Type = OperandType.Constant;
            Parameter = constant;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case OperandType.Constant:
                    return Parameter.ToString();
                
                case OperandType.Memory:
                case OperandType.MemoryLong:
                    return $"0x{Parameter.ToString("X")}";
                    
                case OperandType.RegisterAddress:
                case OperandType.RegisterAddressLong:
                    return $"({Enum.GetName(typeof(Register), Parameter)})";

                case OperandType.RegisterValue:
                case OperandType.RegisterValueLong:
                    return $"{Enum.GetName(typeof(Register), Parameter)}";

                default:
                    throw new InvalidDataException($"{nameof(Type)} is not a valid member of {nameof(OperandType)}!");
            }
        } 
    }
}