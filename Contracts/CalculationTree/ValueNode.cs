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
            return _value; 
        }
    }
}