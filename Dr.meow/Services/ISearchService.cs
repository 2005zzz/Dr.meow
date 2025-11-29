using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models; // ⚠️ 一定要引用 Models，不然它看不懂 SearchResponse

namespace Dr.meow.Services
{
    public interface ISearchService
    {
        // ✅ 關鍵修正：
        // 1. 回傳型別必須是 Task<SearchResponse>
        // 2. 參數必須包含 CancellationToken (因為你的實作有用到)
        Task<SearchResponse> SearchAsync(string keyword, CancellationToken ct = default);
    }
}