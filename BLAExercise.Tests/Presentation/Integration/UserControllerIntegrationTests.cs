using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using BLAExercise.Presentation.Models;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BLAExercise.IntegrationTests.Controllers;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly HttpClient _unauthorizedClient;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly string _jwtToken;

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _userServiceMock = new Mock<IUserService>();

        // Load configuration to get JWTSecretKey
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var jwtSecretKey = configuration
            .GetSection(nameof(ApplicationOptions))
            .GetSection(nameof(ApplicationOptions.JWTSecretKey)).Value;

        _jwtToken = GenerateJwtToken(jwtSecretKey);

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override IUserService with our mock
                services.AddScoped(_ => _userServiceMock.Object);
            });
        }).CreateClient();

        _unauthorizedClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override IUserService with our mock
                services.AddScoped(_ => _userServiceMock.Object);
            });
        }).CreateClient();

        // Set the Authorization header with Bearer token for authenticated requests
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
    }

    // Helper method to generate a JWT token matching your Program.cs configuration
    private string GenerateJwtToken(string secretKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds,
            claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Helper method to serialize objects to JSON
    private static StringContent ToJsonContent(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task Get_Authorized_ReturnsOkWithUsers()
    {
        // Arrange
        var users = CustomFaker.UsersDto.Generate(3);
        _userServiceMock.Setup(s => s.GetAsync(1, 10, nameof(Domain.Models.User.Email), true))
                        .ReturnsAsync(users);

        // Act
        var response = await _client.GetAsync("/api/User?page=1&pageSize=10&sortBy=Email&descendig=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<List<UserDto>>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(users[0].Email, result.Data[0].Email);
    }

    [Fact]
    public async Task Get_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange & Act
        var response = await _unauthorizedClient.GetAsync("/api/User");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Authorized_ReturnsOkWithUser()
    {
        // Arrange
        var user = CustomFaker.UsersDto.Generate();
        _userServiceMock.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var response = await _client.GetAsync($"/api/User/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<UserDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(user.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetById_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var user = CustomFaker.UsersDto.Generate();

        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/User/{user.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByEmail_Authorized_ReturnsOkWithUser()
    {
        // Arrange
        var user = CustomFaker.UsersDto.Generate();
        _userServiceMock.Setup(s => s.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var response = await _client.GetAsync($"/api/User/GetByEmail?email={Uri.EscapeDataString(user.Email)}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<UserDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(user.Email, result.Data.Email);
    }

    [Fact]
    public async Task GetByEmail_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var user = CustomFaker.UsersDto.Generate();

        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/User/GetByEmail?email={Uri.EscapeDataString(user.Email)}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidUserLoginDto_ReturnsCreated()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var userDto = CustomFaker.UsersDto.Generate();
        _userServiceMock.Setup(s => s.AddAsync(It.IsAny<UserLoginDto>())).ReturnsAsync(userDto);

        // Act
        var response = await _unauthorizedClient.PostAsync("/api/User", ToJsonContent(userLoginDto));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<UserDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(userDto.Email, result.Data.Email);
    }

    [Fact]
    public async Task Delete_Authorized_ReturnsNoContent()
    {
        // Arrange
        var userId = CustomFaker.UsersDto.Generate().Id;
        _userServiceMock.Setup(s => s.DeleteAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var response = await _client.DeleteAsync($"/api/User/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var userId = CustomFaker.UsersDto.Generate().Id;

        // Act
        var response = await _unauthorizedClient.DeleteAsync($"/api/User/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_Authorized_ReturnsOk()
    {
        // Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        var updatedUserDto = CustomFaker.UsersDto.Generate();
        _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<UserUpdateDto>())).ReturnsAsync(updatedUserDto);

        // Act
        var response = await _client.PatchAsync("/api/User", ToJsonContent(userUpdateDto));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<UserDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(updatedUserDto.Email, result.Data.Email);
    }

    [Fact]
    public async Task Update_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();

        // Act
        var response = await _unauthorizedClient.PatchAsync("/api/User", ToJsonContent(userUpdateDto));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}