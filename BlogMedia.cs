namespace BlogCMSBackend.Models
{
    public class BlogMedia
    {
        public int Id { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }

        // Foreign key for BlogPost
        public int BlogPostId { get; set; }
        public BlogPost? BlogPost { get; set; }
    }
}