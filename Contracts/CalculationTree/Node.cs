using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.CalculationTree
{
    public abstract class Node
    {
        public abstract Task<string> Evaluate();
        public abstract ICollection<string> GetNodeVariables();
        public abstract Node ReplaceVariable(string variableName, Node node);
    }
}