using System.Collections.Generic;

namespace Contracts.CalculationTree
{
    public abstract class Node
    {
         public abstract string Evaluate();
        public abstract ICollection<string> GetNodeVariables();
        public abstract Node ReplaceVariable(string variableName, Node node);
    }
}