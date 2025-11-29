using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;

namespace Dr.meow.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// 呼叫外部搜尋服務，回傳搜尋結果清單
        /// </summary>
        Task<List<SearchItem>> SearchAsync(string keyword, CancellationToken ct = default);
    }
}
