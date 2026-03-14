namespace BlogApi.DTOs
{
    public class CreateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.Now;
        public List<int> TagIds { get; set; } = [];
    }
}
