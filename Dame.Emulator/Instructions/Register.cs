using System;

namespace Dame.Emulator.Instructions
{
    public enum Register : byte
    {
        A, F, B, C, D, E, H, L,
        AF, BC, DE, HL,
        SP, PC
    }
}
