namespace Dame.Emulator.Instructions
{
    public sealed class Instruction
    {
        public string Name { get; }

        public InstructionDelegate Invoker { get; }

        public Instruction(string name, InstructionDelegate invoker)
        {
            Name = name;
            Invoker = invoker;
        }
    }
}