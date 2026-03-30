using System.Security.Claims;
using BlogApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetArticles(
        int page = 1, int size = 10,
        string? search = null, string? tags = null, string? sort = "published")
    {
        var result = await _service.GetArticlesAsync(page, size, search, tags, sort);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await _service.GetByIdAsync(id);
        return article == null ? NotFound() : Ok(article);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateArticleDto dto)
    {
        var article = await _service.CreateAsync(GetUserId(), dto);
        return Ok(article);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, CreateArticleDto dto)
    {
        var success = await _service.UpdateAsync(id, GetUserId(), dto);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id, GetUserId());
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{articleId}/tags/{tagId}")]
    [Authorize]
    public async Task<IActionResult> RemoveTag(int articleId, int tagId)
    {
        var success = await _service.RemoveTagAsync(articleId, tagId, GetUserId());
        return success ? Ok() : NotFound();
    }
}