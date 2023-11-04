using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.CalculationTree;
using Domain.Helpers.Interfaces;

namespace Domain.Helpers
{
    internal class Parser : IParser
    {
        private static string[] _functions = new string[] { "sum", "avg", "min", "max"};
        private string _expression;
        private int _index;

        public Parser()
        {

        }

        public async Task<Node> ParseAsync(string expression)
        {
            _expression = expression.PrepareExpression();
            _index = 0;
            return await ParseExpression();
        }

        private async Task<Node> ParseExpression()
        {
            bool isNegative = false;
            while (_index < _expression.Length && _expression[_index] == '-')
            {
                isNegative = !isNegative;
                _index++;
            }

            Node left = await ParseTerm();

            if (isNegative)
            {
                left = new OperationNode(new ValueNode("0"), left, '-');
            }
            
            while (_index < _expression.Length &&
                (_expression[_index] == '+' || _expression[_index] == '-'))
            {
                char operation = _expression[_index++];
                Node right = await ParseTerm();

                if (right.GetNodeVariables().Count == 0)
                {
                    right = new ValueNode(await right.Evaluate());
                }

                if (left.GetNodeVariables().Count == 0)
                {
                    left = new ValueNode(await left.Evaluate());
                }

                left = new OperationNode(left, right, operation);   
            }

            return left;
        }

        private async Task<Node> ParseTerm()
        {
            Node left = await ParseFactor();

            while (_index < _expression.Length &&
                (_expression[_index] == '*' || _expression[_index] == '/'))
            {
                char operation = _expression[_index++];
                Node right = await ParseFactor();

                if (right.GetNodeVariables().Count == 0)
                {
                    right = new ValueNode(await right.Evaluate());
                }

                if (left.GetNodeVariables().Count == 0)
                {
                    left = new ValueNode(await left.Evaluate());
                }
                
                left = new OperationNode(left, right, operation);
            }

            return left;
        }

        private async Task<Node> ParseFactor()
        {
            if (_index < _expression.Length && _expression[_index] == '(')
            {
                _index++;
                Node node = await ParseExpression();
                
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
                Node right = await ParseFactor();
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
                    _expression[_index] != '(' &&
                    _expression[_index] != ')')
                {
                    _index++;
                }

                string numberStr = _expression[start_index.._index];

                if (IsFunction(numberStr))
                {
                    return new FunctionNode(await ParseFunctionArguments(), numberStr);
                }
                else if (numberStr == "external_ref")
                {
                    return new ReferenceNode(await ParseReferenceArguments());
                }

                return new ValueNode(numberStr);
            }
            else
            {
                throw new InvalidOperationException("Invalid character.");
            }
        }

        private async Task<Node[]> ParseFunctionArguments()
        {
            if (_expression[_index] != '(')
            {
                throw new InvalidOperationException("Mismatched parentheses");
            }

            List<Node> arguments = new();
            int endOfArguments = EndOfArguments();

            
            Parser argumentsParser = new();
            foreach (string argument in _expression[(_index+1)..(endOfArguments-1)].Split(','))
            {
                arguments.Add(await argumentsParser.ParseAsync(argument));
            }

            _index = endOfArguments;
            return arguments.ToArray();
        }

        private async Task<string> ParseReferenceArguments()
        {
            if (_expression[_index] != '(')
            {
                throw new InvalidOperationException("Mismatched parentheses");
            }

            int endOfArguments = EndOfArguments();
            string link = _expression[(_index+1)..(endOfArguments-1)];
            _index = endOfArguments;

            return link;
        }

        private int EndOfArguments()
        {
            Stack<char> parentheses = new();
            parentheses.Push('(');

            int i = _index+1;
            for (; i < _expression.Length && parentheses.TryPeek(out _); ++i)
            {
                if (_expression[i] == '(')
                {
                    parentheses.Push('(');
                }

                if(_expression[i] == ')')
                {                    
                    if (!parentheses.TryPop(out _))
                    {
                        throw new InvalidOperationException("Mismatched parentheses");
                    }
                }
            }       

            return i;  
        }

        private bool IsFunction(string str)
        {
            return _functions.Contains(str);
        }
    }
}