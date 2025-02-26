namespace Infrastructure.Dtos
{
    public class NewsArticleDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public SourceDto Source { get; set; }
        public string PublishedAt { get; set; }
    }
}
