using System;

namespace Dame.Emulator.Processor
{
    public enum Register : byte
    {
        B, C, D, E, H, L, F, A,
        BC, DE, HL, AF,
        SP, PC
    }
}
