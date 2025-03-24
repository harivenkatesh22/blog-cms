using Microsoft.AspNetCore.Mvc;
using BlogCMSBackend.Data;
using BlogCMSBackend.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BlogCMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly BlogCmsContext _context;

        public SearchController(BlogCmsContext context)
        {
            _context = context;
        }

        // GET: api/Search?query=search+text
        [HttpGet]
        public IActionResult SearchBlogs([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var results = _context.BlogPosts
                .Where(p => p.Published &&
                       ((p.Title != null && p.Title.Contains(query)) ||
                        (p.Content != null && p.Content.Contains(query)) ||
                        (p.SeoTitle != null && p.SeoTitle.Contains(query)) ||
                        (p.SeoDescription != null && p.SeoDescription.Contains(query))
                       ))
                .OrderByDescending(p => p.CreatedDate)
                .ToList();

            return Ok(results);
        }

        // GET: api/Search/seo-suggestions?query=keyword
        [HttpGet("seo-suggestions")]
        public IActionResult GetSeoSuggestions([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Search query cannot be empty.");

                // Search published blog posts that contain the query in either the title, SEO title, or SEO description.
                // We limit the results to 10 items.
                var suggestions = _context.BlogPosts
                    .Where(p => p.Published &&
                           ((p.Title != null && p.Title.Contains(query)) ||
                            (p.SeoTitle != null && p.SeoTitle.Contains(query)) ||
                            (p.SeoDescription != null && p.SeoDescription.Contains(query))
                           ))
                    .Select(p => new {
                        PostId = p.Id,
                        // Return SEO title if available; otherwise fall back to the normal title.
                        Suggestion = !string.IsNullOrWhiteSpace(p.SeoTitle) ? p.SeoTitle : p.Title
                    })
                    .Distinct()
                    .Take(10)
                    .ToList();

                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                // Log exception details (use your logging framework as needed)
                Console.Error.WriteLine("Exception fetching SEO suggestions: " + ex.Message);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
