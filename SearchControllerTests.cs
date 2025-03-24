using Xunit;
using BlogCMSBackend.Controllers;
using BlogCMSBackend.Data;
using BlogCMSBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

public class SearchControllerTests
{
    private BlogCmsContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BlogCmsContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new BlogCmsContext(options);
    }

    [Fact]
    public void SearchBlogs_ReturnsResults_WhenQueryIsProvided()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_SearchBlogs");
        context.BlogPosts.Add(new BlogPost
        {
            Title = "Alpha Article",
            Content = "Some related content.",
            SeoTitle = "Alpha SEO",
            SeoDescription = "Alpha Description",
            Published = true,
            CreatedDate = DateTime.UtcNow
        });
        context.SaveChanges();
        var controller = new SearchController(context);

        // Act
        var result = controller.SearchBlogs("Alpha") as OkObjectResult;
        var results = result?.Value as IEnumerable<BlogPost>;

        // Assert
        Assert.NotNull(results);
        Assert.True(results.Any());
    }

    [Fact]
    public void GetSeoSuggestions_ReturnsSuggestions_WhenQueryIsProvided()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_SeoSuggestions");
        context.BlogPosts.Add(new BlogPost
        {
            Title = "SEO Article",
            SeoTitle = "SEO Featured Title",
            SeoDescription = "SEO Description",
            Published = true,
            CreatedDate = DateTime.UtcNow
        });
        context.SaveChanges();
        var controller = new SearchController(context);

        // Act
        var result = controller.GetSeoSuggestions("SEO") as OkObjectResult;
        var suggestions = result?.Value as IEnumerable<dynamic>;

        // Assert
        Assert.NotNull(suggestions);
        Assert.True(suggestions.Any());
    }
}
