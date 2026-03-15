using BlogApi.Models;

namespace BlogApi.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public AuthorDto Author { get; set; } = null!;
        public List<ArticleTagDto> Tags { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
    }
}
