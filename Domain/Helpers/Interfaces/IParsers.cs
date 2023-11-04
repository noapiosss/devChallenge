using System.Threading.Tasks;
using Contracts.CalculationTree;

namespace Domain.Helpers.Interfaces
{
    public interface IParser
    {
        public Task<Node> ParseAsync(string expresssion);
    }
}