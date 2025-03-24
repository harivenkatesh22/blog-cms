namespace BlogCMSBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string? Country { get; set; }
        public string? Role { get; set; }
        public string? PasswordHash { get; set; } 

        // Navigation property for blog posts
        public ICollection<BlogPost>? BlogPosts { get; set; }
    }
}