using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

public class ArticleService : IArticleService
{
    private readonly BlogDbContext _db;

    public ArticleService(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AllArticlesDto>> GetArticlesAsync(
        int page, int size, string? search, string? tags, string? sort)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = _db.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(term) ||
                a.Content.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var tagList = tags.Split(',')
                .Select(t => t.Trim().ToLower())
                .ToList();

            query = query.Where(a =>
                tagList.All(tag =>
                    a.ArticleTags.Any(at => at.Tag.Name.ToLower() == tag)));
        }

        query = (sort?.ToLower()) switch
        {
            "title" => query.OrderBy(a => a.Title),
            _ => query.OrderByDescending(a => a.PublishedAt)
        };

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(a => new AllArticlesDto
            {
                Id = a.Id,
                Title = a.Title,
                PublishedAt = a.PublishedAt,
                Author = new AuthorDto
                {
                    Id = a.AuthorId,
                    UserName = a.Author.UserName
                }
            })
            .ToListAsync();

        return new PagedResult<AllArticlesDto>(items, total, page, size);
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await _db.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article> CreateAsync(int userId, CreateArticleDto dto)
    {
        var article = new Article
        {
            Title = dto.Title,
            Content = dto.Content,
            ImageUrl = dto.ImageUrl!,
            PublishedAt = dto.PublishedAt,
            AuthorId = userId
        };

        _db.Articles.Add(article);
        await _db.SaveChangesAsync();

        if (dto.TagIds != null && dto.TagIds.Any())
        {
            foreach (var tagId in dto.TagIds)
            {
                var exists = await _db.Set<ArticleTag>()
                    .AnyAsync(at => at.ArticleId == article.Id && at.TagId == tagId);

                if (!exists)
                {
                    _db.Set<ArticleTag>().Add(new ArticleTag
                    {
                        ArticleId = article.Id,
                        TagId = tagId
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

        return article;
    }

    public async Task<bool> UpdateAsync(int id, int userId, CreateArticleDto dto)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null) return false;

        if (article.AuthorId != userId)
            throw new UnauthorizedAccessException();

        article.Title = dto.Title;
        article.Content = dto.Content;
        article.ImageUrl = dto.ImageUrl!;
        article.PublishedAt = dto.PublishedAt;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null) return false;

        if (article.AuthorId != userId)
            throw new UnauthorizedAccessException();

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveTagAsync(int articleId, int tagId, int userId)
    {
        var article = await _db.Articles.FindAsync(articleId);
        if (article == null) return false;

        if (article.AuthorId != userId)
            throw new UnauthorizedAccessException();

        var articleTag = await _db.Set<ArticleTag>()
            .FirstOrDefaultAsync(at => at.ArticleId == articleId && at.TagId == tagId);

        if (articleTag == null) return false;

        _db.Remove(articleTag);
        await _db.SaveChangesAsync();

        return true;
    }
}