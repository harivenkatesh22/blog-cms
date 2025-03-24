using System.Collections.Generic;

namespace BlogCMSBackend.DTOs
{
    public class BlogPostDto
    {
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public string? SeoTitle { get; set; } = string.Empty;
        public string? SeoDescription { get; set; } = string.Empty;
        // Optional media attachments.
        public List<MediaDto>? Media { get; set; }
        public int ClickCount { get; set; }
        // Added Author information
        public string? Author { get; set; }
    }
}