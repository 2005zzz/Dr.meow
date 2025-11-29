using System.Collections.Generic; // 記得引用這個，才能用 List

namespace WebApp.Models // (請確認這跟你的專案 namespace 一致)
{
    public class RagQueryRequest
    {
        // ⚠️ 修正 1: 改為 "Query" 以對應後端 WebAPI 的定義
        public string Query { get; set; } = "";
    }

    public class RagQueryResponse
    {
        public string Answer { get; set; } = "";

        // ⚠️ 修正 2: 新增 Sources 列表，用來接後端傳回來的引用文件
        public List<RagSourceDoc> Sources { get; set; } = new List<RagSourceDoc>();
    }

    // ⚠️ 修正 3: 新增這個類別，用來定義單一引用來源的結構
    public class RagSourceDoc
    {
        public string FileName { get; set; } = "";
        public string Page { get; set; } = "";
        public string Content { get; set; } = "";
        public double Score { get; set; }
    }

    // (如果你還有 SearchItem 或 SearchResponse 等 UI 用的類別，可以保留在下方)
    public class SearchResponse
    {
        public string Answer { get; set; } = "";
        public List<SearchItem> Items { get; set; } = new List<SearchItem>();
    }

    public class SearchItem
    {
        public string Title { get; set; } = "";
        public string Snippet { get; set; } = "";
        public string Url { get; set; } = "";
        public string Source { get; set; } = "";
        public DateTime? ModifiedAt { get; set; }
    }
}