using System.Linq.Expressions;
using Dame.Processor;

namespace Dame.Instructions
{
    interface IInstructionContext
    {
        ProcessorFlags FlagsRead { get; set; }

        ParameterExpression FlagsVariable { get; }
    }
}