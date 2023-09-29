using System;
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
            float leftValue = float.Parse(_left.Evaluate());
            float rightValue = float.Parse(_right.Evaluate());

            switch (_operation)
            {
                case '+':
                    return (leftValue + rightValue).ToString();
                case '-':
                    return (leftValue - rightValue).ToString();
                case '*':
                    return (leftValue * rightValue).ToString();
                case '/':
                    if (rightValue == 0)
                    {
                        throw new DivideByZeroException("Division by zero.");
                    }
                    return (leftValue / rightValue).ToString();;
                default:
                    throw new InvalidOperationException("Invalid operation.");
            }
        }
    }
}