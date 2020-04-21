using System.Linq.Expressions;
using Dame.Processor;

namespace Dame.Instructions
{
    public interface IInstructionContext
    {
        ProcessorFlags FlagsRead { get; set; }

        ParameterExpression FlagsVariable { get; }
    }
}