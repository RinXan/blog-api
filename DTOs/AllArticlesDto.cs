using BlogApi.Models;

namespace BlogApi.DTOs
{
    public class AllArticlesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public AuthorDto Author { get; set; } = null!;
    }
}
