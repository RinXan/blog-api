using System.Security.Claims;
using BlogApi.DTOs;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentsController(ICommentService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException();
        return int.Parse(claim.Value);
    }

    [HttpPost("{articleId}")]
    [Authorize]
    public async Task<IActionResult> CreateComment(int articleId, CommentDto dto)
    {
        var comment = await _service.CreateAsync(GetUserId(), articleId, dto.Text);

        return Ok(new
        {
            message = "Comment added successfully!",
            id = comment.Id,
            text = comment.Text,
            username = comment.Author.UserName,
            authorId = comment.AuthorId
        });
    }

    [HttpGet("article/{articleId}")]
    public async Task<IActionResult> GetCommentsByArticle(int articleId)
    {
        return Ok(await _service.GetByArticleAsync(articleId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComment(int id)
    {
        var comment = await _service.GetByIdAsync(id);
        return comment == null ? NotFound() : Ok(comment);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var success = await _service.DeleteAsync(id, GetUserId());
        return success ? Ok(new { message = "Deleted successfully!" }) : NotFound();
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(int id, CommentDto dto)
    {
        var success = await _service.UpdateAsync(id, GetUserId(), dto.Text);
        return success ? Ok(new { message = "Updated successfully!" }) : NotFound();
    }
}