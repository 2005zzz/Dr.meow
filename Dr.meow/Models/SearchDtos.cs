namespace Dr.meow.Models
{
    public class SearchItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Source { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? Snippet { get; set; }
    }

    public class SearchResponse
    {
        public string? Answer { get; set; }
        public List<SearchItem> Items { get; set; } = new();
    }
}
