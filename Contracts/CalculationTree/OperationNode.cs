using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Contracts.CalculationTree
{
    public class OperationNode : Node
    {
        private Node _left;
        private Node _right;
        private char _operation;

        public OperationNode(Node left, Node right, char operation)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

        public override string Evaluate()
        {
            string leftValue = _left.Evaluate();
            string rightValue = _right.Evaluate();

            switch (_operation)
            {
                case '+':
                    return (float.Parse(leftValue, CultureInfo.InvariantCulture) + float.Parse(rightValue, CultureInfo.InvariantCulture)).ToString();
                case '-':
                    return (float.Parse(leftValue, CultureInfo.InvariantCulture) - float.Parse(rightValue, CultureInfo.InvariantCulture)).ToString();
                case '*':
                    return (float.Parse(leftValue, CultureInfo.InvariantCulture) * float.Parse(rightValue, CultureInfo.InvariantCulture)).ToString();
                case '/':
                    if (rightValue == "0")
                    {
                        throw new DivideByZeroException("Division by zero.");
                    }
                    return (float.Parse(leftValue, CultureInfo.InvariantCulture) / float.Parse(rightValue, CultureInfo.InvariantCulture)).ToString();;
                default:
                    throw new InvalidOperationException("Invalid operation.");
            }
        }

        public override ICollection<string> GetNodeVariables()
        {
            List<string> nodeValues = new();

            if (_left is not null)
            {
                nodeValues.AddRange(_left.GetNodeVariables());
            }
            if (_right is not null)
            { 
                nodeValues.AddRange(_right.GetNodeVariables());
            }

            return nodeValues;
        }

        public override Node ReplaceVariable(string variableName, Node node)
        {
            if (_left is not null)
            {
                _left = _left.ReplaceVariable(variableName, node);
            }
            if (_right is not null)
            {             
                _right = _right.ReplaceVariable(variableName, node);
            }

            return this;
        }
    }
}