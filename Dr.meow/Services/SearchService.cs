using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Dr.meow.Services
{
    public class SearchService : ISearchService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public SearchService(HttpClient http, IConfiguration config)
        {
            _http = http;
            // 從 appsettings.json 讀取 RAG API 的 URL
            _baseUrl = config["ExternalSearch:BaseUrl"] ?? string.Empty;
        }

        /// <summary>
        /// 擴充後的搜尋方法
        /// </summary>
        /// <param name="keyword">使用者輸入的內容</param>
        /// <param name="mode">當前模式：consult(問答), request(需求單), vulnerability(變更單)</param>
        /// <param name="ct"></param>
        public async Task<SearchResponse> SearchAsync(string keyword, string mode = "consult", CancellationToken ct = default)
        {
            // 1. 檢查設定
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return new SearchResponse { Answer = "尚未設定 ExternalSearch:BaseUrl。" };
            }

            try
            {
                // 2. 建立請求內容 (Payload)
                // 加入 Mode 參數，讓後端 AI 知道現在是要「專業問答」還是「引導填表」
                var requestPayload = new
                {
                    Query = keyword,
                    Mode = mode, // 傳入當前模式
                    Timestamp = DateTime.Now
                };

                // 3. 呼叫後端 API
                // 建議路徑對齊後端 Controller 設定
                var response = await _http.PostAsJsonAsync(
                    $"{_baseUrl}/api/rag/ask",
                    requestPayload,
                    ct
                );

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    return new SearchResponse
                    {
                        Answer = $"RAG API 連線失敗 ({(int)response.StatusCode}): {body}"
                    };
                }

                // 4. 解析 JSON 結果 (使用 RagResponse DTO)
                var ragResult = await response.Content.ReadFromJsonAsync<RagResponse>(cancellationToken: ct);

                if (ragResult == null)
                {
                    return new SearchResponse { Answer = "後端回傳空值。" };
                }

                // 5. 轉換為前端 UI 用的 SearchResponse
                var uiResult = new SearchResponse
                {
                    // AI 的回答內容
                    Answer = ragResult.Answer ?? "(無回答)",
                    Items = new List<SearchItem>()
                };

                // 6. 將 Sources (引用來源) 轉換為 SearchItem (側邊欄卡片)
                // 增加 null 檢查與防呆
                if (ragResult.Sources != null && ragResult.Sources.Any())
                {
                    foreach (var src in ragResult.Sources)
                    {
                        // 確保至少有一頁內容可以預覽
                        string previewSnippet = "無預覽內容";
                        if (src.Pages != null && src.Pages.Count > 0 && src.PageContents != null)
                        {
                            var firstPageKey = src.Pages[0];
                            if (src.PageContents.ContainsKey(firstPageKey))
                            {
                                previewSnippet = src.PageContents[firstPageKey];
                            }
                        }

                        uiResult.Items.Add(new SearchItem
                        {
                            Title = src.FileName ?? "未命名文件",
                            Snippet = previewSnippet,
                            Url = "#",
                            Source = "Dr.Meow 知識庫",
                            ModifiedAt = DateTime.Now,
                            Pages = src.Pages,
                            PageContents = src.PageContents
                        });
                    }
                }

                return uiResult;
            }
            catch (Exception ex)
            {
                return new SearchResponse
                {
                    Answer = $"搜尋服務發生例外錯誤：{ex.Message}"
                };
            }
        }
    }
}