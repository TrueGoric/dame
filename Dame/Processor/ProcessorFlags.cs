namespace Dame.Processor
{
    enum ProcessorFlags : byte
    {
        Zero = 1 << 7,
        Arithmetic = 1 << 6, // 0 for add, 1 for sub
        HalfCarry = 1 << 5,
        Carry = 1 << 4,
        All = Zero | Arithmetic | HalfCarry | Carry,
        None = 0
    }
}