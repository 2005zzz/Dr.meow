namespace Dr.meow.Models
{
    public class RagQueryResponse
    {
        public string Answer { get; set; }
        public List<RagSourceItem>? Sources { get; set; }
    }

    public class RagSourceItem
    {
        public string Title { get; set; }
        public string Snippet { get; set; }
        public double Score { get; set; }
        public string Url { get; set; }
    }
}
