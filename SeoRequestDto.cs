using System;

namespace BlogCMSBackend.DTOs
{
    public class SeoRequestDto
    {
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? Content { get; set; }
    }
}
