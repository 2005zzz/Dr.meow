using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic; // 確保引用 List

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

        public async Task<SearchResponse> SearchAsync(string keyword, CancellationToken ct = default)
        {
            // 1. 檢查設定
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return new SearchResponse { Answer = "尚未設定 ExternalSearch:BaseUrl。" };
            }

            try
            {
                // 2. 建立請求 (使用匿名物件即可，只要屬性名稱對得上後端)
                // 後端模型是: public class RagQueryRequest { public string Query { get; set; } }
                var requestPayload = new { Query = keyword };

                // 3. 呼叫後端 API
                // 注意路徑：通常是 /api/rag/ask (依據你後端 Controller 的 Route 設定)
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

                // 4. 解析 JSON 結果 (使用前端定義的 RagResponse DTO)
                // 這裡會自動把後端的 { Answer, Sources } 對應進來
                var ragResult = await response.Content.ReadFromJsonAsync<RagResponse>(cancellationToken: ct);

                if (ragResult == null)
                {
                    return new SearchResponse { Answer = "後端回傳空值。" };
                }

                // 5. 轉換為前端 UI 用的 SearchResponse
                var uiResult = new SearchResponse
                {
                    Answer = ragResult.Answer ?? "(無回答)",
                    Items = new List<SearchItem>()
                };

                // 6. 將 Sources (引用來源) 轉換為 SearchItem (側邊欄卡片)
                if (ragResult.Sources != null)
                {
                    foreach (var src in ragResult.Sources)
                    {
                        uiResult.Items.Add(new SearchItem
                        {
                            // 組合標題：檔名 + 頁數
                            Title = $"{src.FileName} (Page {src.Page})",

                            // 內容：顯示原始文件片段
                            Snippet = src.Content,

                            // 連結：暫時用 #，未來可做成檔案下載連結
                            Url = "#",

                            // 來源標籤
                            Source = "RAG Knowledge Base",

                            ModifiedAt = DateTime.Now // 或是 null
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