using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Contracts.CalculationTree
{
    public class FunctionNode : Node
    {
        private Node[] _arguments;
        private string _function;

        public FunctionNode(Node[] arguments, string function)
        {
            _arguments = arguments;
            _function = function;
        }

        public override string Evaluate()
        {
            string[] argumentValues = new string[_arguments.Length];

            for (int i = 0; i < _arguments.Length; ++i)
            {
                argumentValues[i] = _arguments[i].Evaluate();
            }

            switch (_function)
            {
                case "sum":
                    return Sum(argumentValues);
                case "avg":
                    return Avg(argumentValues);
                case "min":
                    return Min(argumentValues);;
                case "max":
                    return Max(argumentValues);;
                default:
                    throw new InvalidOperationException("Invalid function.");
            }
        }

        private string Sum(string[] arguments)
        {
            float result = 0;

            foreach (string argument in arguments)
            {
                result += float.Parse(argument, CultureInfo.InvariantCulture);
            }

            return result.ToString(CultureInfo.InvariantCulture);
        }

        private string Avg(string[] arguments)
        {
            float result = 0;

            foreach (string argument in arguments)
            {
                result += float.Parse(argument, CultureInfo.InvariantCulture);
            }

            result /= arguments.Length;
            return result.ToString(CultureInfo.InvariantCulture);
        }

        private string Min(string[] arguments)
        {
            float result = float.MaxValue;

            foreach (string argument in arguments)
            {
                result = Math.Min(result, float.Parse(argument, CultureInfo.InvariantCulture));
            }

            return result.ToString(CultureInfo.InvariantCulture);
        }

        private string Max(string[] arguments)
        {
            float result = float.MinValue;

            foreach (string argument in arguments)
            {
                result = Math.Max(result, float.Parse(argument, CultureInfo.InvariantCulture));
            }

            return result.ToString(CultureInfo.InvariantCulture);
        }

        public override ICollection<string> GetNodeVariables()
        {
            List<string> nodeValues = new();

            foreach (Node argument in _arguments)
            {
                nodeValues.AddRange(argument.GetNodeVariables());
            }

            return nodeValues;
        }

        public override Node ReplaceVariable(string variableName, Node node)
        {
            for (int i = 0; i < _arguments.Length; ++i)
            {
                _arguments[i] = _arguments[i].ReplaceVariable(variableName, node);
            }

            return this;
        }
    }
}