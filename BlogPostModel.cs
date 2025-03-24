namespace BlogCMSBackend.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Published { get; set; }
        public int ClickCount { get; set; }

        // Foreign key for Author
        public int AuthorId { get; set; }
        public User? Author { get; set; }

        // Navigation property for media
        public ICollection<BlogMedia>? BlogMedias { get; set; }
    }
}