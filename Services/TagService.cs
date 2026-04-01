using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class TagService : ITagService
{
    private readonly BlogDbContext _db;

    public TagService(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Tag>> GetTagsByArticleAsync(int articleId)
    {
        var tags = await _db.ArticleTags
            .Where(at => at.ArticleId == articleId)
            .Select(at => at.Tag)
            .ToListAsync();

        if (!tags.Any())
            throw new KeyNotFoundException($"No tags found for article {articleId}");

        return tags;
    }

    public async Task<Tag> GetByIdWithArticlesAsync(int id)
    {
        var tag = await _db.Tags
            .Include(t => t.ArticleTags)
                .ThenInclude(at => at.Article)
                .ThenInclude(a => a.Author)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        return tag;
    }

    public async Task<Tag> GetOrCreateAsync(string name)
    {
        var tag = await _db.Tags
            .FirstOrDefaultAsync(t => t.Name == name);

        if (tag != null)
            return tag;

        tag = new Tag { Name = name };

        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        return tag;
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
    }
}