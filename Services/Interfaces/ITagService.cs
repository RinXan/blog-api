using BlogApi.Models;

namespace BlogApi.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetTagsByArticleAsync(int articleId);
        Task<Tag?> GetByIdWithArticlesAsync(int id);
        Task<Tag> GetOrCreateAsync(string name);
        Task DeleteAsync(int id);
    }
}
