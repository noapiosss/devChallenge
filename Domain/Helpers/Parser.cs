using System;
using System.Collections.Generic;
using Contracts.CalculationTree;
using Contracts.Database;

namespace Domain.Helpers
{
    internal class Parser
    {
        public Dictionary<string, Cell> Cells;
        private readonly Dictionary<string, int> _indexes;
        private readonly HashSet<string> _currentlyEvaluating;

        public Parser(Dictionary<string, Cell> cells)
        {
            Cells = cells;
            _indexes = new();
            _currentlyEvaluating = new();
        }

        public Node Parse(string cellId)
        {
            if (!Cells.ContainsKey(cellId))
            {
                throw new NullReferenceException("There is no such variable.");
            }
            
            if (!Cells[cellId].IsExpression)
            {
                return new ValueNode(Cells[cellId].Value);
            }

            _indexes[cellId] = 0;

            if (!_currentlyEvaluating.Add(cellId))
            {
                throw new InvalidCastException("Self linking."); 
            }

            Cells[cellId].Value = Cells[cellId].Value.PrepareExpression();
            Node result = ParseExpression(cellId);
            _currentlyEvaluating.Remove(cellId);

            return result;
        }

        private Node ParseExpression(string cellId)
        {
            Node left = ParseTerm(cellId);
            
            while (_indexes[cellId] < Cells[cellId].Value.Length &&
                (Cells[cellId].Value[_indexes[cellId]] == '+' || Cells[cellId].Value[_indexes[cellId]] == '-'))
            {
                char operation = Cells[cellId].Value[_indexes[cellId]++];
                Node right = ParseTerm(cellId);
                left = new OperationNode(left, right, operation);
            }

            return new ValueNode(left.Evaluate());
        }

        private Node ParseTerm(string cellId)
        {
            Node left = ParseFactor(cellId);

            while (_indexes[cellId] < Cells[cellId].Value.Length &&
                (Cells[cellId].Value[_indexes[cellId]] == '*' || Cells[cellId].Value[_indexes[cellId]] == '/'))
            {
                char operation = Cells[cellId].Value[_indexes[cellId]++];
                Node right = ParseFactor(cellId);
                left = new OperationNode(left, right, operation);
            }

            return new ValueNode(left.Evaluate());
        }

        private Node ParseFactor(string cellId)
        {
            if (_indexes[cellId] < Cells[cellId].Value.Length && Cells[cellId].Value[_indexes[cellId]] == '(')
            {
                _indexes[cellId]++;
                Node node = ParseExpression(cellId);
                
                if (_indexes[cellId] >= Cells[cellId].Value.Length || Cells[cellId].Value[_indexes[cellId]] != ')')
                {
                    throw new InvalidOperationException("Mismatched parentheses.");
                }

                _indexes[cellId]++;
                return node;
            }
            else if (char.IsDigit(Cells[cellId].Value[_indexes[cellId]]) || char.IsLetter(Cells[cellId].Value[_indexes[cellId]]))
            {
                int start_index = _indexes[cellId];
                while (_indexes[cellId] < Cells[cellId].Value.Length &&
                    Cells[cellId].Value[_indexes[cellId]] != '+' &&
                    Cells[cellId].Value[_indexes[cellId]] != '-' &&
                    Cells[cellId].Value[_indexes[cellId]] != '*' &&
                    Cells[cellId].Value[_indexes[cellId]] != '/' &&
                    Cells[cellId].Value[_indexes[cellId]] != ')')
                {
                    _indexes[cellId]++;
                }

                string numberStr = Cells[cellId].Value[start_index.._indexes[cellId]];

                foreach (char c in numberStr)
                {
                    if (!char.IsDigit(c))
                    {
                        return Parse(numberStr);
                    }
                }

                return new ValueNode(numberStr);
            }
            else
            {
                throw new InvalidOperationException("Invalid character.");
            }
        }
    }
}