using System;

namespace Dr.meow.Models
{
    /// <summary>
    /// 前端要用的單一搜尋結果項目
    /// </summary>
    public class SearchItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Source { get; set; }
        public string? Snippet { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
