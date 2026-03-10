using System.Security.Claims;
using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly BlogDbContext _db;

    public CommentsController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpPost("{articleId}")]
    [Authorize]
    public async Task<ActionResult<Comment>> CreateComment(int articleId, CreateCommentDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var article = await _db.Articles.FindAsync(articleId);

        if (article == null)
            return NotFound("Article not found");

        var comment = new Comment
        {
            Text = dto.Text,
            ArticleId = articleId,
            AuthorId = userId,
            CreatedAt = DateTime.Now
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var fullComment = await _db.Comments
        .Include(c => c.Author)
        .FirstOrDefaultAsync(c => c.Id == comment.Id);

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, new
        {
            id = comment.Id,
            text = comment.Text,
            username = fullComment.Author.UserName,
            authorId = comment.AuthorId
        });
    }

    [HttpGet("article/{articleId}")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByArticle(int articleId)
    {
        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.ArticleId == articleId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> GetComment(int id)
    {
        var comment = await _db.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return NotFound();

        return Ok(comment);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _db.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return NotFound("Comment not found");
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (comment.AuthorId != userId && comment.Article.AuthorId != userId)
            return Unauthorized("You can only delete your comments or comments on your articles");

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return Ok("Deleted successfully!");
    }
}
