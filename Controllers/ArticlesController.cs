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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Article>> CreateArticle(CreateArticleDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var article = new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                PublishedAt = dto.PublishedAt,
                AuthorId = userId,
            };
            
            _db.Articles.Add(article);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticles), new { id = article.Id }, article);
        }
    }
}
