using System;
using Contracts.CalculationTree;
using Domain.Helpers.Interfaces;

namespace Domain.Helpers
{
    internal class Parser : IParser
    {
        private string _expression;
        private int _index;

        public Parser()
        {

        }

        public Node Parse(string expression)
        {
            _expression = expression.PrepareExpression();
            _index = 0;
            return ParseExpression();
        }

        private Node ParseExpression()
        {
            bool isNegative = false;
            while (_index < _expression.Length && _expression[_index] == '-')
            {
                isNegative = !isNegative;
                _index++;
            }

            Node left = ParseTerm();

            if (isNegative)
            {
                left = new OperationNode(new ValueNode("0"), left, '-');
            }
            
            while (_index < _expression.Length &&
                (_expression[_index] == '+' || _expression[_index] == '-'))
            {
                char operation = _expression[_index++];
                Node right = ParseTerm();

                if (right.GetNodeVariables().Count == 0)
                {
                    right = new ValueNode(right.Evaluate());
                }

                if (left.GetNodeVariables().Count == 0)
                {
                    left = new ValueNode(left.Evaluate());
                }

                left = new OperationNode(left, right, operation);   
            }

            return left;
        }

        private Node ParseTerm()
        {
            Node left = ParseFactor();

            while (_index < _expression.Length &&
                (_expression[_index] == '*' || _expression[_index] == '/'))
            {
                char operation = _expression[_index++];
                Node right = ParseFactor();

                if (right.GetNodeVariables().Count == 0)
                {
                    right = new ValueNode(right.Evaluate());
                }

                if (left.GetNodeVariables().Count == 0)
                {
                    left = new ValueNode(left.Evaluate());
                }
                
                left = new OperationNode(left, right, operation);
            }

            return left;
        }

        private Node ParseFactor()
        {
            if (_index < _expression.Length && _expression[_index] == '(')
            {
                _index++;
                Node node = ParseExpression();
                
                if (_index >= _expression.Length || _expression[_index] != ')')
                {
                    throw new InvalidOperationException("Mismatched parentheses.");
                }

                _index++;
                return node;
            }

            if (_expression[_index] == '+' || _expression[_index] == '-')
            {
                char opration = _expression[_index++];
                Node right = ParseFactor();
                return new OperationNode(new ValueNode("0"), right, opration);
            }

            else if (char.IsDigit(_expression[_index]) || char.IsLetter(_expression[_index]))
            {
                int start_index = _index;
                while (_index < _expression.Length &&
                    _expression[_index] != '+' &&
                    _expression[_index] != '-' &&
                    _expression[_index] != '*' &&
                    _expression[_index] != '/' &&
                    _expression[_index] != ')')
                {
                    _index++;
                }

                string numberStr = _expression[start_index.._index];
                return new ValueNode(numberStr);
            }
            else
            {
                throw new InvalidOperationException("Invalid character.");
            }
        }
    }
}