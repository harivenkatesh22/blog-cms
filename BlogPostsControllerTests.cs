using Xunit;
using BlogCMSBackend.Controllers;
using BlogCMSBackend.Data;
using BlogCMSBackend.Models;
using BlogCMSBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System;

public class BlogPostsControllerTests
{
    private BlogCmsContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BlogCmsContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new BlogCmsContext(options);
    }
 
    [Fact]
    public void GetPublishedPosts_ReturnsOnlyPublishedPosts()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_PublishedPosts");
        // Seed an author.
        var author = new User 
        {
            Email = "author@example.com",
            FirstName = "Author",
            LastName = "User",
            Country = "India",
            Role = "Blogger"
        };
        context.Users.Add(author);
        context.SaveChanges();
 
        context.BlogPosts.Add(new BlogPost 
        { 
            Title = "Published Post", 
            Content = "This is a published post.", 
            Published = true, 
            CreatedDate = DateTime.UtcNow,
            AuthorId = author.Id
        });
        context.SaveChanges();
 
        var controller = new BlogPostsController(context);
 
        // Act
        var result = controller.GetPublishedPosts() as OkObjectResult;
        var posts = result?.Value as IEnumerable<dynamic>;
 
        // Assert
        Assert.NotNull(posts);
        Assert.Single(posts);
    }
 
    [Fact]
    public void CreatePost_CreatesNewBlogPost()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_CreatePost");
 
        // Seed a user with a specific ID.
        var user = new User 
        {
            Id = 1,
            Email = "blogger@example.com", 
            FirstName = "Blogger", 
            LastName = "User", 
            Country = "India", 
            Role = "Blogger"
        };
        context.Users.Add(user);
        context.SaveChanges();
 
        var controller = new BlogPostsController(context);
 
        // Set HttpContext with proper claims.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "Blogger")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
 
        var postDto = new BlogPostDto
        {
            Title = "New Blog Post",
            Content = "Content for the new blog post.",
            SeoTitle = "SEO Title",
            SeoDescription = "SEO Description",
            ClickCount = 0,
            Media = null  // No media attachments.
        };
 
        // Act
        var result = controller.CreatePost(postDto) as OkObjectResult;
        Assert.NotNull(result);

        // Use reflection to retrieve the "Message" property.
        var type = result?.Value?.GetType() ?? typeof(string);

        var messageProp = type.GetProperty("Message");
        Assert.NotNull(messageProp);
        var messageValue = messageProp?.GetValue(result?.Value, null) as string;
 
        // Assert
        Assert.Equal("Blog post created successfully.", messageValue);
        Assert.Single(context.BlogPosts);
    }
 
    [Fact]
    public void UpdatePost_UpdatesExistingPost()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_UpdatePost");
        // Seed a user.
        var user = new User 
        {
            Email = "updater@example.com", 
            FirstName = "Updater", 
            LastName = "User", 
            Country = "India", 
            Role = "Blogger"
        };
        context.Users.Add(user);
        context.SaveChanges();
 
        // Add a post authored by this user.
        var post = new BlogPost 
        { 
            Title = "Old Title", 
            Content = "Old content", 
            Published = true, 
            CreatedDate = DateTime.UtcNow,
            AuthorId = user.Id 
        };
        context.BlogPosts.Add(post);
        context.SaveChanges();
 
        var controller = new BlogPostsController(context);
 
        // Set HttpContext with proper claims for this user.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "Blogger")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
 
        var postDto = new BlogPostDto
        {
            Title = "Updated Title",
            Content = "Updated content",
            SeoTitle = "Updated SEO Title",
            SeoDescription = "Updated SEO Description",
            ClickCount = post.ClickCount,
            Media = null
        };
 
        // Act
        var result = controller.UpdatePost(post.Id, postDto) as OkObjectResult;
 
        // Reload post from the database and assert.
        var updatedPost = context.BlogPosts.Find(post.Id);
        Assert.NotNull(updatedPost);
        Assert.Equal("Updated Title", updatedPost.Title);
        Assert.Equal("Updated content", updatedPost.Content);
    }
}
