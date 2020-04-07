namespace Dame.Instructions
{
    delegate void InstructionDelegate();

    delegate T InstructionValue<T>()
        where T : unmanaged;
    
    delegate void InstructionAction();
    delegate void InstructionFunction<T>(T value)
        where T : unmanaged;
}