namespace BlogApi.DTOs
{
    public class ArticleTagDto
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }
        public TagDto Tag { get; set; } = null!;
    }

    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
