using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;
using Microsoft.Extensions.Configuration;

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
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return new SearchResponse
                {
                    Answer = "尚未設定 ExternalSearch:BaseUrl。"
                };
            }

            try
            {
                // 🔥 建立 RAG 請求物件
                var request = new RagQueryRequest
                {
                    Question = keyword
                };

                // 🔥 呼叫後端 RAG API（POST）
                var response = await _http.PostAsJsonAsync(
                    $"{_baseUrl}/api/rag/query",
                    request,
                    ct
                );

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    return new SearchResponse
                    {
                        Answer = $"RAG API 呼叫失敗 ({(int)response.StatusCode}): {body}"
                    };
                }

                // 🔥 解析 JSON 結果
                var rag = await response.Content.ReadFromJsonAsync<RagQueryResponse>(cancellationToken: ct);

                if (rag == null)
                {
                    return new SearchResponse { Answer = "後端沒回傳內容。" };
                }

                // 🔥 組成前端要的格式
                var result = new SearchResponse
                {
                    Answer = rag.Answer ?? "",
                    Items = new List<SearchItem>()
                };

                if (rag.Sources != null)
                {
                    foreach (var s in rag.Sources)
                    {
                        result.Items.Add(new SearchItem
                        {
                            Title = s.Title,
                            Snippet = s.Snippet,
                            Url = s.Url,
                            Source = "RAG",   // 你要顯示來源名字也可以改
                            ModifiedAt = null
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new SearchResponse
                {
                    Answer = $"搜尋服務連線失敗：{ex.Message}"
                };
            }
        }
    }
}
