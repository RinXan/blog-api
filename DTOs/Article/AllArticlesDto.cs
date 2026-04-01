using BlogApi.DTOs.User;
using BlogApi.Models;

namespace BlogApi.DTOs.Article
{
    public class AllArticlesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public AuthorDto Author { get; set; } = null!;
    }
}
