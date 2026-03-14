using System.Security.Claims;
using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly BlogDbContext _db;
        public TagsController(BlogDbContext db)
        {
            _db = db;
        }
        
        [HttpGet("article/{id}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByArtilce(int id)
        {
            var article = await _db.Articles
                .Where(a => a.Id == id)
                .Include(at => at.ArticleTags)
                .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync();

            if (article == null) return NotFound(new {message = $"article with id {id} not found"});

            var tags = article.ArticleTags.Select(at => at.Tag);

            return Ok(tags);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetArticlesByTag(int id) 
        {
            var tag = await _db.Tags
                .Include(t => t.ArticleTags)
                .ThenInclude(at => at.Article)
                .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) return NotFound(new {message = "Tag not found"});

            return Ok(tag);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Tag>> GetOrCreateTag(CreateTagDto dto)
        {
            var tagExist = await _db.Tags.FirstOrDefaultAsync(t => t.Name == dto.Name);

            if (tagExist == null)
            {
                tagExist = new Tag
                {
                    Name = dto.Name,
                };

                _db.Tags.Add(tagExist);
                await _db.SaveChangesAsync();
            }

            return Ok(tagExist);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Tag>> DeleteTag(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (userId == null) return Unauthorized(new { message = "You are not authorized!" });

            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) return NotFound(new { message = $"Tag #{id} not found" });

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();

            return Ok(new {message = $"Tag: #{tag.Name} deleted!"});
        }
    }
}
