using Microsoft.AspNetCore.Mvc;
using BlogCMSBackend.DTOs;
using System;
using System.Collections.Generic;

namespace BlogCMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeoController : ControllerBase
    {
        // POST: api/Seo/suggest
        [HttpPost("suggest")]
        public IActionResult GetSeoSuggestions([FromBody] SeoRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request payload.");
            }

            var suggestions = new List<string>();

            // SEO Title Suggestions
            if (string.IsNullOrWhiteSpace(request.SeoTitle) || request.SeoTitle.Length < 5)
                suggestions.Add("Enhance your SEO Title by including relevant keywords and making it at least 5 characters long.");

            // SEO Description Suggestions
            if (string.IsNullOrWhiteSpace(request.SeoDescription) || request.SeoDescription.Length < 50)
                suggestions.Add("Your SEO Description is too short. Consider providing at least 50 characters.");

            // Content-based Suggestions
            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                int wordCount = request.Content.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount < 100)
                    suggestions.Add("Your content is brief. Consider adding more detailed content to improve SEO.");
            }
            else
            {
                suggestions.Add("Content is missing. A comprehensive article improves search rankings.");
            }

            // Return the suggestions in a structured response
            return Ok(new { Suggestions = suggestions });
        }
    }
}
