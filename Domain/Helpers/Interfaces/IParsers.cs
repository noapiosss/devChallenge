using Contracts.CalculationTree;

namespace Domain.Helpers.Interfaces
{
    public interface IParser
    {
        public Node Parse(string expresssion);
    }
}