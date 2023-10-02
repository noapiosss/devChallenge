using System.Collections.Generic;

namespace Contracts.CalculationTree
{
    public class ValueNode : Node
    {
        private string _value;

        public ValueNode(string value)
        {
            _value = value;
        }

        public override string Evaluate()
        {
            return string.IsNullOrEmpty(_value) ? "0" : _value; 
        }

        public override ICollection<string> GetNodeVariables()
        {
            if (!string.IsNullOrEmpty(_value) && !char.IsDigit(_value[0]))
            {
                return new List<string>() {_value};
            }
            
            return new List<string>();
        }

        public override Node ReplaceVariable(string variableName, Node node)
        {
            if (!string.IsNullOrEmpty(_value) && _value == variableName)
            {
                return node;
            }

            return this;
        }
    }
}