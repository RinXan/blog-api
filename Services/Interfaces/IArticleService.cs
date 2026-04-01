using BlogApi.DTOs.Article;
using BlogApi.DTOs.Common;
using BlogApi.Models;

public interface IArticleService
{
    Task<PagedResult<AllArticlesDto>> GetArticlesAsync(
        int page,
        int size,
        string? search,
        string? tags,
        string? sort);

    Task<Article?> GetByIdAsync(int id);

    Task<Article> CreateAsync(int userId, CreateArticleDto dto);

    Task<bool> UpdateAsync(int id, int userId, CreateArticleDto dto);

    Task<bool> DeleteAsync(int id, int userId);

    Task<bool> RemoveTagAsync(int articleId, int tagId, int userId);
}