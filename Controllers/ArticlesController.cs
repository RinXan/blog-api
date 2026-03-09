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
    public class ArticlesController : ControllerBase
    {
        private readonly BlogDbContext _db;

        public ArticlesController(BlogDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            var articles = await _db.Articles.ToListAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _db.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Article>> CreateArticle(CreateArticleDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var article = new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl!,
                PublishedAt = dto.PublishedAt,
                AuthorId = userId,
            };

            _db.Articles.Add(article);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticles), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Article>> UpdateArticle(int id, CreateArticleDto dto)
        {
            var article = await _db.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (article.AuthorId != userId) return Forbid("You can only edit your own articles");

            article.Title = dto.Title;
            article.Content = dto.Content;
            article.ImageUrl = dto.ImageUrl!;
            article.PublishedAt = dto.PublishedAt;

            await _db.SaveChangesAsync();
            return Ok("Updated successfully!");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Article>> DeleteArticle(int id)
        {
            var article = await _db.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (article.AuthorId != userId) return Forbid("You can only delete your own articles");

            _db.Articles.Remove(article);
            await _db.SaveChangesAsync();

            return Ok("Deleted successfully!");
        }
    }
}
