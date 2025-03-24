using Microsoft.EntityFrameworkCore;
using BlogCMSBackend.Models;

namespace BlogCMSBackend.Data
{
    public class BlogCmsContext : DbContext
    {
        public BlogCmsContext(DbContextOptions<BlogCmsContext> options) : base(options) { }

        // DbSets for various entities
        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogMedia> BlogMedias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // BlogPost - User relationship
            modelBuilder.Entity<BlogPost>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // BlogMedia - BlogPost relationship
            modelBuilder.Entity<BlogMedia>()
                .HasOne(m => m.BlogPost)
                .WithMany(p => p.BlogMedias)
                .HasForeignKey(m => m.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
