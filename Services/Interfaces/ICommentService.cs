using BlogApi.Models;

namespace BlogApi.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetByArticleAsync(int articleId);
        Task<Comment> CreateAsync(int userId, int articleId, string text);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> UpdateAsync(int id, int userId, string text);
    }
}
