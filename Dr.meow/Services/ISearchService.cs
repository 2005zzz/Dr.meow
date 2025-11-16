using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;

namespace Dr.meow.Services
{
    public interface ISearchService
    {
        Task<SearchResponse> SearchAsync(string keyword, CancellationToken ct = default);
    }
}
