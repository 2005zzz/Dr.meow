using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models; // ✅ 確保引用 Models 取得 SearchResponse

namespace Dr.meow.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// 智慧搜尋與 AI 代理服務
        /// </summary>
        /// <param name="keyword">使用者輸入的查詢或需求描述</param>
        /// <param name="mode">當前模式：consult (預設問答), request (需求單), vulnerability (變更單)</param>
        /// <param name="ct">非同步取消權杖</param>
        /// <returns>包含 AI 回答與參考來源的 SearchResponse</returns>
        Task<SearchResponse> SearchAsync(string keyword, string mode = "consult", CancellationToken ct = default);
    }
}