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
        private readonly string _apiKey;

        public SearchService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ExternalSearch:BaseUrl"] ?? string.Empty;
            _apiKey = config["ExternalSearch:ApiKey"] ?? string.Empty;
        }

        public async Task<SearchResponse> SearchAsync(string keyword, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                // 還沒設定 BaseUrl 的友善提示
                return new SearchResponse
                {
                    Answer = "尚未在 appsettings.json 設定 ExternalSearch:BaseUrl。"
                };
            }

            try
            {
                // 這裡先假設你同學的 API 是 GET ?keyword=...
                var url = $"{_baseUrl}?keyword={Uri.EscapeDataString(keyword)}";

                using var req = new HttpRequestMessage(HttpMethod.Get, url);

                if (!string.IsNullOrEmpty(_apiKey))
                {
                    // 若對方用別的 header（例如 api-key），再改這一行就好
                    req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_apiKey}");
                }

                using var resp = await _http.SendAsync(req, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync(ct);
                    return new SearchResponse
                    {
                        Answer = $"外部 API 呼叫失敗（{(int)resp.StatusCode}）：{body}"
                    };
                }

                // 假設對方回傳的 json 結構長得跟 SearchResponse 一樣
                var data = await resp.Content.ReadFromJsonAsync<SearchResponse>(cancellationToken: ct);
                return data ?? new SearchResponse { Answer = "外部 API 沒有傳回內容。" };
            }
            catch (Exception ex)
            {
                // 連不到、port 沒開、DNS 錯誤 都會進這裡
                return new SearchResponse
                {
                    Answer = $"外部搜尋服務無法連線：{ex.Message}"
                };
            }
        }
    }
}
