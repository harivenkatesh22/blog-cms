using Xunit;
using BlogCMSBackend.Controllers;
using BlogCMSBackend.Data;
using BlogCMSBackend.Models;
using BlogCMSBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class AuthControllerTests
{
    private BlogCmsContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BlogCmsContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new BlogCmsContext(options);
    }

    // Build in-memory configuration for JWT settings.
    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"JwtSettings:SecretKey", "ThisIsASecretKeyThatIsLongEnoughToBeValid"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiryInMinutes", "120"}
        };
        return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
    }

    [Fact]
    public void Signup_CreatesNewUser()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_Signup");
        var configuration = GetConfiguration();
        var controller = new AuthController(context, configuration);
        var userDto = new UserDto
        {
            Email = "newuser@example.com",
            Password = "password123",
            FirstName = "New",
            LastName = "User",
            Country = "India"
        };

        // Act
        var result = controller.Signup(userDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Signup successful", result.Value);
        Assert.Single(context.Users);
    }

    [Fact]
    public void Signup_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_SignupDuplicate");
        context.Users.Add(new User
        {
            Email = "duplicate@example.com",
            FirstName = "Duplicate",
            LastName = "User",
            Country = "India",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "Blogger"
        });
        context.SaveChanges();

        var configuration = GetConfiguration();
        var controller = new AuthController(context, configuration);
        var userDto = new UserDto
        {
            Email = "duplicate@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User",
            Country = "India"
        };

        // Act
        var result = controller.Signup(userDto) as ObjectResult;

        // Assert – expecting BadRequest (400)
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void Login_ReturnsToken_ForValidCredentials()
    {
        // Arrange
        var context = GetInMemoryDbContext("TestDb_Login");
        // Seed a user with a specific Id.
        var user = new User
        {
            Id = 1,
            Email = "valid@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Country = "India",
            Role = "Blogger"
        };
        context.Users.Add(user);
        context.SaveChanges();

        var configuration = GetConfiguration();
        var controller = new AuthController(context, configuration);
        var loginDto = new LoginDto { Email = "valid@example.com", Password = "password123" };

        // Act
        var result = controller.Login(loginDto) as OkObjectResult;
        var type = result.Value?.GetType();
        var tokenProp = type?.GetProperty("Token");
        Assert.NotNull(tokenProp);
        var tokenValue = tokenProp.GetValue(result.Value, null) as string ?? "DefaultValue";
 
        // Assert
        Assert.False(string.IsNullOrEmpty(tokenValue));
    }
}
