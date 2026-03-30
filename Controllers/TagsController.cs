using System.Security.Claims;
using BlogApi.DTOs;
using BlogApi.Models;
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
        return tags == null
            ? NotFound(new { message = $"article with id {id} not found" })
            : Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticlesByTag(int id)
    {
        var tag = await _service.GetByIdWithArticlesAsync(id);
        return tag == null
            ? NotFound(new { message = "Tag not found" })
            : Ok(tag);
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

        var success = await _service.DeleteAsync(id);

        return success
            ? Ok(new { message = $"Tag #{id} deleted!" })
            : NotFound(new { message = $"Tag #{id} not found" });
    }
}