namespace Dr.meow.Models
{
    // RAG 檢索結果的結構
    public class SearchItem
    {
        // 確保這些屬性可以被序列化（例如 JSON）

        // 檢索到的文件片段或最終答案的內容
        public string? Content { get; set; }

        // 引用的文件標題或名稱
        public string? SourceTitle { get; set; }

        // 文件在向量資料庫或文件庫中的識別碼
        public string? DocumentId { get; set; }
    }
}