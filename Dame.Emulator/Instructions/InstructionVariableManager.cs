using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dame.Emulator.Instructions
{
    public class InstructionVariableManager
    {
        private Dictionary<string, ParameterExpression> variables = new Dictionary<string, ParameterExpression>();

        public ParameterExpression Get<T>(string name)
            where T : unmanaged
            => variables.ContainsKey(name)
                ? variables[name]
                : variables[name] = Expression.Variable(typeof(T), name);
    }
}