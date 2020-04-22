using System.Linq.Expressions;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Instructions
{
    public interface IInstructionContext
    {
        ProcessorFlags FlagsRead { get; set; }

        ParameterExpression FlagsVariable { get; }
    }
}