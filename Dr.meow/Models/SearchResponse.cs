namespace Dr.meow.Models
{

    public class SearchResponse
    {
        public string? Answer { get; set; }
        public List<SearchItem> Items { get; set; } = new();
    }
}


