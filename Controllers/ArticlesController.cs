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
        public async Task<ActionResult<PagedResult<ArticleDto>>> GetArticles(int page = 1, int size = 10)
        {

            if (page < 1) page = 1;
            if (size < 1 || size > 100) size = 10;

            int total = await _db.Articles.CountAsync();

            var articles = await _db.Articles
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Include(x => x.Author)
                .Include(x => x.ArticleTags)!
                    .ThenInclude(at => at.Tag)!
                    .Select(x => new ArticleDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Content = x.Content,
                        PublishedAt = x.PublishedAt,
                        Author = new AuthorDto
                        {
                            Id = x.Author.Id,
                            UserName = x.Author.UserName,
                            Email = x.Author.Email,
                        },
                        Tags = x.ArticleTags.Select(at => new ArticleTagDto
                        {
                            ArticleId = at.ArticleId,
                            TagId = at.TagId,
                            Tag = new TagDto
                            {
                                Id = at.Tag.Id,
                                Name = at.Tag.Name
                            }
                        }).ToList()
                    })
                .ToListAsync();

            return Ok(new PagedResult<ArticleDto>(articles, total, page, size));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _db.Articles
                .Include(a => a.Author)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Include(a => a.Comments)
                    .ThenInclude(a => a.Author)
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

            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var tId in dto.TagIds)
                {
                    var tag = await _db.Tags.FindAsync(tId);
                    if (tag == null) continue;

                    bool exists = await _db.Set<ArticleTag>()
                        .AnyAsync(at => at.ArticleId == article.Id && at.TagId == tId);
                    if (exists) continue;

                    var articleTag = new ArticleTag
                    {
                        ArticleId = article.Id,
                        TagId = tId
                    };

                    _db.Set<ArticleTag>().Add(articleTag);
                }
                await _db.SaveChangesAsync();
            }

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

        [HttpDelete("{articleId}/tags/{tagId}")]
        [Authorize]
        public async Task<IActionResult> RemoveTagFromArticle(int articleId, int tagId)
        {
            var article = await _db.Articles
                .Include(a => a.Author)
                .Where(a => a.Id == articleId)
                .FirstOrDefaultAsync();

            if (article == null) return NotFound(new { message = $"Article #{articleId} not found" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (article.AuthorId != userId) return Unauthorized(new { message = "You cannot delete tags from others article" });

            var articleTag = await _db.Set<ArticleTag>()
                .FirstOrDefaultAsync(at => at.ArticleId == articleId && at.TagId == tagId);

            if (articleTag == null) return NotFound(new { message = $"Tag #{tagId} does not exist" });

            _db.Set<ArticleTag>().Remove(articleTag);
            await _db.SaveChangesAsync();

            return Ok(new { messgae = "Tag removed from artilce!" });
        }
    } 
}
