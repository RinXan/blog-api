using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly BlogDbContext _db;

    public CommentService(BlogDbContext db, ILogger<CommentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        var comment = await _db.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment ?? throw new KeyNotFoundException("Comment not found");
    }

    public async Task<IEnumerable<Comment>> GetByArticleAsync(int articleId)
    {
        return await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.ArticleId == articleId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> CreateAsync(int userId, int articleId, string text)
    {
        var article = await _db.Articles.FindAsync(articleId);
        if (article == null)
            throw new Exception("Article not found");

        var comment = new Comment
        {
            Text = text,
            ArticleId = articleId,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return await _db.Comments
            .Include(c => c.Author)
            .FirstAsync(c => c.Id == comment.Id);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var comment = await _db.Comments
            .Include(c => c.Article)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if (comment.AuthorId != userId && comment.Article.AuthorId != userId)
            throw new UnauthorizedAccessException();

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(int id, int userId, string text)
    {
        var comment = await _db.Comments.FindAsync(id);

        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if (comment.AuthorId != userId)
            throw new UnauthorizedAccessException();

        comment.Text = text;
        comment.CreatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return true;
    }
}