using BlogApi.DTOs.User;

namespace BlogApi.DTOs.Article
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
        public List<BlogApi.Models.Comment> Comments { get; set; } = new();
    }
}
