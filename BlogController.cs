using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BlogCMSBackend.Models;
using BlogCMSBackend.Data;
using BlogCMSBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BlogCMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly BlogCmsContext _context;

        public BlogPostsController(BlogCmsContext context)
        {
            _context = context;
        }

        // GET: api/BlogPosts/published
        [HttpGet("published")]
        [AllowAnonymous]
        public IActionResult GetPublishedPosts()
        {
            var posts = _context.BlogPosts
                .Include(p => p.Author) // Eagerly load Author
                .Include(p => p.BlogMedias) // Eagerly load BlogMedias
                .Where(p => p.Published)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.SeoTitle,
                    p.SeoDescription,
                    p.CreatedDate,
                    p.ClickCount,
                    Author = p.Author != null ? (p.Author.FirstName + " " + p.Author.LastName) : "Unknown",
                    Media = p.BlogMedias != null && p.BlogMedias.Any()
                        ? p.BlogMedias.Select(m => new { m.MediaUrl, m.MediaType })
                        : Enumerable.Empty<object>()
                })
                .ToList();

            return Ok(posts);
        }

        // GET: api/BlogPosts/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetPost(int id)
        {
            var post = _context.BlogPosts
                .Include(p => p.Author) // Eagerly load Author
                .Include(p => p.BlogMedias) // Eagerly load BlogMedias
                .FirstOrDefault(p => p.Id == id && p.Published);

            if (post == null)
                return NotFound("Blog post not found or not published.");

            // Increment click count.
            post.ClickCount++;
            _context.SaveChanges();

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.SeoTitle,
                post.SeoDescription,
                post.CreatedDate,
                post.UpdatedDate,
                Author = post.Author != null ? (post.Author.FirstName + " " + post.Author.LastName) : "Unknown",
                Media = post.BlogMedias != null && post.BlogMedias.Any()
                    ? post.BlogMedias.Select(m => new { m.MediaUrl, m.MediaType })
                    : Enumerable.Empty<object>()
            });
        }

        // POST: api/BlogPosts
[HttpPost]
[Authorize(Roles = "Blogger,Admin")]
public IActionResult CreatePost([FromBody] BlogPostDto postDto)
{
    if (postDto == null || string.IsNullOrWhiteSpace(postDto.Title) || string.IsNullOrWhiteSpace(postDto.Content))
    {
        return BadRequest("Title and Content are required.");
    }

    try
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID is missing."));
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Create the new blog post
        var blogPost = new BlogPost
        {
            Title = postDto.Title.Trim(),
            Content = postDto.Content.Trim(),
            SeoTitle = string.IsNullOrWhiteSpace(postDto.SeoTitle) ? GenerateSeoTitle(postDto.Title.Trim()) : postDto.SeoTitle.Trim(),
            SeoDescription = string.IsNullOrWhiteSpace(postDto.SeoDescription) ? GenerateSeoDescription(postDto.Content.Trim()) : postDto.SeoDescription.Trim(),
            AuthorId = userId,
            CreatedDate = DateTime.UtcNow,
            Published = userRole == "Admin", // Auto-publish for Admins, else require moderation
            ClickCount = postDto.ClickCount, // ClickCount will be 0 by default unless specified
            BlogMedias = postDto.Media != null 
                ? postDto.Media.Select(m => new BlogMedia
                {
                    MediaUrl = m.MediaUrl,
                    MediaType = m.MediaType
                }).ToList()
                : new List<BlogMedia>()
        };

        // If author information is provided, store it separately for logging or debugging purposes
        if (!string.IsNullOrWhiteSpace(postDto.Author))
        {
            Console.WriteLine($"Author Info (provided by client): {postDto.Author}");
        }

        _context.BlogPosts.Add(blogPost);
        _context.SaveChanges();

        return Ok(new { Message = "Blog post created successfully.", blogPost.Id });
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Error creating blog post: " + ex.Message);
        return StatusCode(500, "An unexpected error occurred while creating the blog post.");
    }
}


        // PUT: api/BlogPosts/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Blogger,Admin")]
        public IActionResult UpdatePost(int id, [FromBody] BlogPostDto postDto)
        {
            if (postDto == null)
                return BadRequest("Invalid post data.");

            var post = _context.BlogPosts.Include(p => p.BlogMedias).FirstOrDefault(p => p.Id == id);
            if (post == null)
                return NotFound("Blog post not found.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not authenticated.");
            int userId = int.Parse(userIdClaim);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (post.AuthorId != userId && userRole != "Admin")
                return Forbid("You are not allowed to update this post.");

            post.Title = postDto.Title?.Trim() ?? post.Title;
            post.Content = postDto.Content ?? post.Content;
            post.SeoTitle = string.IsNullOrWhiteSpace(postDto.SeoTitle)
                ? GenerateSeoTitle(post.Title ?? string.Empty)
                : postDto.SeoTitle;
            post.SeoDescription = string.IsNullOrWhiteSpace(postDto.SeoDescription)
                ? GenerateSeoDescription(post.Content ?? string.Empty)
                : postDto.SeoDescription;
            post.UpdatedDate = DateTime.UtcNow;

            if (postDto.Media != null)
            {
                var existingMedia = _context.BlogMedias.Where(m => m.BlogPostId == post.Id);
                _context.BlogMedias.RemoveRange(existingMedia);
                post.BlogMedias = postDto.Media.Select(m => new BlogMedia
                {
                    MediaUrl = m.MediaUrl ?? string.Empty,
                    MediaType = m.MediaType ?? string.Empty
                }).ToList();
            }

            _context.SaveChanges();
            return Ok(new { Message = "Blog post updated successfully.", post.Id });
        }

        // --- Helper Methods ---
        private string GenerateSeoTitle(string title) => (title ?? string.Empty).Trim();

        private string GenerateSeoDescription(string content)
        {
            content = content ?? string.Empty;
            return content.Length > 150 ? content.Substring(0, 150) + "..." : content;
        }
    }
}