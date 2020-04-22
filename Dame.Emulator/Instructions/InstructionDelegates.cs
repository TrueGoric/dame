namespace Dame.Emulator.Instructions
{
    public delegate void InstructionDelegate();

    public delegate T InstructionValue<T>()
        where T : unmanaged;
    
    public delegate void InstructionAction();
    public delegate void InstructionFunction<T>(T value)
        where T : unmanaged;
}