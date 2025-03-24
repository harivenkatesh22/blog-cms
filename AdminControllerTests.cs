using Xunit;
using BlogCMSBackend.Controllers;
using BlogCMSBackend.Data;
using BlogCMSBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public class AdminControllerTests
{
    private BlogCmsContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BlogCmsContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new BlogCmsContext(options);
    }

    // Helper: Set HttpContext with Admin role.
    private void SetAdminContext(ControllerBase controller)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin")
        }, "TestAuth"));
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [Fact]
    public void GetUnapprovedPosts_ReturnsNonPublishedPosts()
    {
        // Arrange - Seed an author and an unapproved post.
        var context = GetInMemoryDbContext("TestDb_UnapprovedPosts");
        var author = new User
        {
            Id = 1,
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
            Title = "Unapproved Post", 
            Published = false, 
            CreatedDate = System.DateTime.UtcNow,
            AuthorId = author.Id
        });
        context.SaveChanges();
        // Use AsNoTracking to force fresh read.
        var controller = new AdminController(context);
        SetAdminContext(controller);
 
        // Act
        var result = controller.GetUnapprovedPosts() as OkObjectResult;
        var posts = result?.Value as IEnumerable<dynamic>;
 
        // Assert
        Assert.NotNull(posts);
        Assert.True(posts.Any(), "No unapproved posts were returned.");
    }

    [Fact]
    public void ApprovePost_SetsPublishedTrue()
    {
        // Arrange - Seed an author and a pending post.
        var context = GetInMemoryDbContext("TestDb_ApprovePost");
        var author = new User
        {
            Id = 1,
            Email = "author@example.com",
            FirstName = "Author",
            LastName = "User",
            Country = "India",
            Role = "Blogger"
        };
        context.Users.Add(author);
        context.SaveChanges();

        var post = new BlogPost 
        { 
            Title = "Pending Post", 
            Published = false, 
            CreatedDate = System.DateTime.UtcNow,
            AuthorId = author.Id
        };
        context.BlogPosts.Add(post);
        context.SaveChanges();
 
        var controller = new AdminController(context);
        SetAdminContext(controller);
 
        // Act
        var result = controller.ApprovePost(post.Id) as OkObjectResult;
        // Retrieve fresh data from the database.
        var updatedPost = context.BlogPosts.AsNoTracking().FirstOrDefault(x => x.Id == post.Id);
 
        // Assert
        Assert.NotNull(updatedPost);
        Assert.True(updatedPost.Published, "ApprovePost did not set Published to true.");
    }
}
