using System;

namespace Dame.Emulator.Instructions
{
    public enum OperandType
    {
        Empty,

        RegisterValue,
        RegisterValueLong,
        RegisterAddress,
        RegisterAddressLong,

        Memory,
        MemoryLong,
        
        Constant // used by test bit, reset bit, set bit and interrupt call instructions
    }
}
