namespace BlogApi.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public DateTime PublishedAt {  get; set; }
        public virtual User Author { get; set; } = null!;
    }
}
