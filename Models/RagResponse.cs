// 檔案位置: Dr.meow/Models/RagResponse.cs

namespace Dr.meow.Models
{
    // 這是最終回傳給前端的完整物件
    public class RagResponse
    {
        public string Answer { get; set; } = "";
        public List<RagSourceDoc> Sources { get; set; } = new List<RagSourceDoc>();
    }

    // 這是每一個引用來源的詳細資料
    public class RagSourceDoc
    {
        public string FileName { get; set; } = "";
        public string Page { get; set; } = "";
        public string Content { get; set; } = ""; // 原始文件內容
        public double Score { get; set; }         // 相關度分數 (選用)
    }
}