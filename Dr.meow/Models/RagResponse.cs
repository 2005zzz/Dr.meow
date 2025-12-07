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
        public string FileName { get; set; }
        public List<string> Pages { get; set; } = new();
        public Dictionary<string, string> PageContents { get; set; } = new();
        public double Score { get; set; }
    }

}