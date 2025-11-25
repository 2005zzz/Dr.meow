using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dr.meow.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dr.meow.Services
{
    public class SearchService : ISearchService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SearchService> _logger;
        private readonly string _baseUrl;

        public SearchService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<SearchService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // appsettings.json 的 ExternalSearch:BaseUrl
            _baseUrl = configuration["ExternalSearch:BaseUrl"] ?? string.Empty;
        }

        public async Task<List<SearchItem>> SearchAsync(string keyword, CancellationToken ct = default)
        {
            var results = new List<SearchItem>();

            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                _logger.LogWarning("ExternalSearch:BaseUrl 未設定。");
                return results;
            }

            // 這裡的 /api/search 要改成你同學實際的路徑
            var url = $"{_baseUrl.TrimEnd('/')}/api/search?keyword={Uri.EscapeDataString(keyword)}";

            try
            {
                using var resp = await _httpClient.GetAsync(url, ct);
                resp.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                await using var stream = await resp.Content.ReadAsStreamAsync(ct);
                var data = await JsonSerializer.DeserializeAsync<List<SearchItem>>(stream, options, ct);

                return data ?? results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "呼叫外部搜尋服務失敗。");
                return results;
            }
        }
    }
}
