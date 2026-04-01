using System.Security.Claims;
using BlogApi.DTOs.Tag;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;

    public TagsController(ITagService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new UnauthorizedAccessException();
        return int.Parse(claim.Value);
    }

    [HttpGet("article/{id}")]
    public async Task<IActionResult> GetTagsByArticle(int id)
    {
        var tags = await _service.GetTagsByArticleAsync(id);
        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticlesByTag(int id)
    {
        return Ok(await _service.GetByIdWithArticlesAsync(id));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetOrCreateTag(CreateTagDto dto)
    {
        var tag = await _service.GetOrCreateAsync(dto.Name);
        return Ok(tag);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTag(int id)
    {
        GetUserId(); 

        await _service.DeleteAsync(id);

        return Ok(new { message = $"Tag #{id} deleted!" });
    }
}