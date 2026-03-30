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
        var article = await _db.Articles
            .Where(a => a.Id == articleId)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync();

        if (article == null)
            return null!;

        return article.ArticleTags.Select(at => at.Tag);
    }

    public async Task<Tag?> GetByIdWithArticlesAsync(int id)
    {
        return await _db.Tags
            .Include(t => t.ArticleTags)
                .ThenInclude(at => at.Article)
                .ThenInclude(a => a.Author)
            .FirstOrDefaultAsync(t => t.Id == id);
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

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id);
        if (tag == null) return false;

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();

        return true;
    }
}