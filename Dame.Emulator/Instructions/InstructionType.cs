using System;

namespace Dame.Emulator.Instructions
{
    public enum InstructionType : byte
    {
        Nop, Stop, Halt,

        Add, AddCarry, Subtract, SubtractCarry,
        Increment, Decrement,
        Xor, Or, And, Compare,

        RotateLeft, RotateLeftCarry, RotateRight, RotateRightCarry,
        ShiftLeft, ShiftRight, ShiftRightLogical,
        Swap, SetBit, ResetBit, TestBit,

        Load, Push, Pop,
        
        Jump, JumpZ, JumpNZ, JumpC, JumpNC,
        JumpRelative, JumpRelativeZ, JumpRelativeNZ, JumpRelativeC, JumpRelativeNC,
        Call, CallZ, CallNZ, CallC, CallNC,
        Return, ReturnZ, ReturnNZ, ReturnC, ReturnNC,
        EnableInterrupts, DisableInterrupts, ReturnEnableInterrupts,

        DAA, Complement, ComplementCarry, SetCarry
    }
}
