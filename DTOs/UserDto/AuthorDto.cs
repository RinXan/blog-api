using System.Text.Json.Serialization;
using BlogApi.Models;

namespace BlogApi.DTOs.User
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
