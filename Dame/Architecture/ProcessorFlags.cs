namespace Dame.Architecture
{
    enum ProcessorFlags : byte
    {
        Zero = 1 << 7,
        Arithmetic = 1 << 6, // 0 for add, 1 for sub
        NibbleCarry = 1 << 5,
        Carry = 1 << 4
    }
}