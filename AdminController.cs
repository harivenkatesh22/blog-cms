using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogCMSBackend.Models;
using BlogCMSBackend.Data;
using System.Linq;

namespace BlogCMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly BlogCmsContext _context;

        public AdminController(BlogCmsContext context)
        {
            _context = context;
        }

   // Modified `GetUnapprovedPosts` to address the issue
        [HttpGet("unapprovedPosts")]
        public IActionResult GetUnapprovedPosts()
        {
         var posts = _context.BlogPosts
        .Include(p => p.Author)
        .Include(p => p.BlogMedias)
        .Where(p => !p.Published)
        .Select(p => new
        {
            p.Id,
            p.Title,
            p.CreatedDate,
            Author = p.Author != null ? $"{p.Author.FirstName} {p.Author.LastName}" : "Unknown",
            Media = p.BlogMedias != null && p.BlogMedias.Any()
                ? p.BlogMedias.Select(m => new { m.MediaUrl, m.MediaType })
                : Enumerable.Empty<object>()
        })
        .ToList();

    return Ok(posts);
}

        // POST: api/Admin/approvePost/{postId}
        [HttpPost("approvePost/{postId}")]
        public IActionResult ApprovePost(int postId)
        {
            var post = _context.BlogPosts
                .Include(p => p.Author)
                .FirstOrDefault(p => p.Id == postId);

            if (post == null)
                return NotFound("Blog post not found.");

            post.Published = true;
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Blog post approved and published.",
                post.Id,
                post.Title,
                Author = post.Author != null ? $"{post.Author.FirstName} {post.Author.LastName}" : "Unknown"
            });
        }

        // GET: api/Admin/users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.Country,
                u.Role
            }).ToList();

            return Ok(users);
        }

        // PUT: api/Admin/updateUserRole/{userId}
        [HttpPut("updateUserRole/{userId}")]
        public IActionResult UpdateUserRole(int userId, [FromBody] string newRole)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            user.Role = newRole;
            _context.SaveChanges();

            return Ok(new { Message = "User role updated.", user.Id, user.Role });
        }
    }
}
