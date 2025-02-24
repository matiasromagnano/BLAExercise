using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
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

public class SneakerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly HttpClient _unauthorizedClient;
    private readonly Mock<ISneakerService> _sneakerServiceMock;

    public SneakerControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _sneakerServiceMock = new Mock<ISneakerService>();

        // Load configuration to get JWTSecretKey
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var jwtSecretKey = configuration
            .GetSection(nameof(ApplicationOptions))
            .GetSection(nameof(ApplicationOptions.JWTSecretKey)).Value;

        var jwtToken = GenerateJwtToken(jwtSecretKey);

        // Create authenticated client
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _sneakerServiceMock.Object);
            });
        }).CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        // Create unauthenticated client
        _unauthorizedClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _sneakerServiceMock.Object);
            });
        }).CreateClient();
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

    private static StringContent ToJsonContent(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task GetById_Authorized_ReturnsOkWithSneaker()
    {
        // Arrange
        var sneaker = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(s => s.GetByIdAsync(sneaker.Id)).ReturnsAsync(sneaker);

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/{sneaker.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<SneakerDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(sneaker.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetById_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var sneaker = CustomFaker.SneakersDto.Generate();

        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/Sneaker/{sneaker.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_SneakerNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = CustomFaker.SneakersDto.Generate().Id;
        _sneakerServiceMock.Setup(s => s.GetByIdAsync(id))
                           .ThrowsAsync(new NotFoundException($"Sneaker with Id: '{id}' was not found"));

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Sneaker with Id: '{id}' was not found", result.Message);
    }

    [Fact]
    public async Task GetByUserId_Authorized_ReturnsOkWithSneakers()
    {
        // Arrange
        var userId = CustomFaker.User.Id;
        var sneakers = CustomFaker.SneakersDto.Generate(2);
        _sneakerServiceMock.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(sneakers);

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/GetByUserId?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<List<SneakerDto>>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(sneakers[0].Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetByUserId_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var userId = CustomFaker.User.Id;

        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/Sneaker/GetByUserId?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByUserId_NoSneakersFound_ReturnsNotFound()
    {
        // Arrange
        var userId = CustomFaker.User.Id;
        _sneakerServiceMock.Setup(s => s.GetByUserIdAsync(userId))
                           .ThrowsAsync(new NotFoundException($"No Sneakers found for UserId: '{userId}'"));

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/GetByUserId?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No Sneakers found for UserId: '{userId}'", result.Message);
    }

    // 3. Test Cases for GET /api/Sneaker/GetByUserEmail
    [Fact]
    public async Task GetByUserEmail_Authorized_ReturnsOkWithSneakers()
    {
        // Arrange
        var email = CustomFaker.User.Email;
        var sneakers = CustomFaker.SneakersDto.Generate(2);
        _sneakerServiceMock.Setup(s => s.GetByUserEmailAsync(email)).ReturnsAsync(sneakers);

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/GetByUserEmail?email={Uri.EscapeDataString(email)}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<List<SneakerDto>>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(sneakers[0].Id, result.Data[0].Id);
    }

    [Fact]
    public async Task GetByUserEmail_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var email = CustomFaker.User.Email;

        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/Sneaker/GetByUserEmail?email={Uri.EscapeDataString(email)}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByUserEmail_NoSneakersFound_ReturnsNotFound()
    {
        // Arrange
        var email = CustomFaker.User.Email;
        _sneakerServiceMock.Setup(s => s.GetByUserEmailAsync(email))
                           .ThrowsAsync(new NotFoundException($"No Sneakers found for User Email: '{email}'"));

        // Act
        var response = await _client.GetAsync($"/api/Sneaker/GetByUserEmail?email={Uri.EscapeDataString(email)}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"No Sneakers found for User Email: '{email}'", result.Message);
    }

    [Fact]
    public async Task Get_Authorized_ReturnsOkWithSneakers()
    {
        // Arrange
        var sneakers = CustomFaker.SneakersDto.Generate(3);
        _sneakerServiceMock.Setup(s => s.GetAsync(1, 10, nameof(Domain.Models.Sneaker.Year), true))
                           .ReturnsAsync(sneakers);

        // Act
        var response = await _client.GetAsync("/api/Sneaker?page=1&pageSize=10&sortBy=Year&descendig=true");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<List<SneakerDto>>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(sneakers[0].Id, result.Data[0].Id);
    }

    [Fact]
    public async Task Get_Unauthorized_ReturnsUnauthorized()
    {
        // Act
        var response = await _unauthorizedClient.GetAsync("/api/Sneaker");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_NoSneakersFound_ReturnsNotFound()
    {
        // Arrange
        _sneakerServiceMock.Setup(s => s.GetAsync(1, 10, nameof(Domain.Models.Sneaker.Year), true))
                           .ThrowsAsync(new NotFoundException("No Sneakers were found"));

        // Act
        var response = await _client.GetAsync("/api/Sneaker?page=1&pageSize=10&sortBy=Year&descendig=true");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No Sneakers were found", result.Message);
    }

    [Fact]
    public async Task Create_Authorized_ReturnsCreated()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(s => s.AddAsync(It.IsAny<SneakerCreateDto>())).ReturnsAsync(sneakerDto);

        // Act
        var response = await _client.PostAsync("/api/Sneaker", ToJsonContent(sneakerCreateDto));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<SneakerDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(sneakerDto.Id, result.Data.Id);
    }

    [Fact]
    public async Task Create_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();

        // Act
        var response = await _unauthorizedClient.PostAsync("/api/Sneaker", ToJsonContent(sneakerCreateDto));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Authorized_ReturnsNoContent()
    {
        // Arrange
        var sneakerId = CustomFaker.SneakersDto.Generate().Id;
        _sneakerServiceMock.Setup(s => s.DeleteAsync(sneakerId)).Returns(Task.CompletedTask);

        // Act
        var response = await _client.DeleteAsync($"/api/Sneaker/{sneakerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var sneakerId = CustomFaker.SneakersDto.Generate().Id;

        // Act
        var response = await _unauthorizedClient.DeleteAsync($"/api/Sneaker/{sneakerId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_SneakerNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = CustomFaker.SneakersDto.Generate().Id;
        _sneakerServiceMock.Setup(s => s.DeleteAsync(id))
                           .ThrowsAsync(new NotFoundException($"Sneaker with Id: '{id}' was not found"));

        // Act
        var response = await _client.DeleteAsync($"/api/Sneaker/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Sneaker with Id: '{id}' was not found", result.Message);
    }

    [Fact]
    public async Task Update_Authorized_ReturnsOk()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        var updatedSneakerDto = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(s => s.UpdateAsync(It.IsAny<SneakerUpdateDto>())).ReturnsAsync(updatedSneakerDto);

        // Act
        var response = await _client.PatchAsync("/api/Sneaker", ToJsonContent(sneakerUpdateDto));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<SneakerDto>>(content);
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(updatedSneakerDto.Id, result.Data.Id);
    }

    [Fact]
    public async Task Update_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();

        // Act
        var response = await _unauthorizedClient.PatchAsync("/api/Sneaker", ToJsonContent(sneakerUpdateDto));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_SneakerNotFound_ReturnsNotFound()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        _sneakerServiceMock.Setup(s => s.UpdateAsync(It.IsAny<SneakerUpdateDto>()))
                           .ThrowsAsync(new NotFoundException($"Sneaker with Id: '{sneakerUpdateDto.Id}' was not found"));

        // Act
        var response = await _client.PatchAsync("/api/Sneaker", ToJsonContent(sneakerUpdateDto));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Sneaker with Id: '{sneakerUpdateDto.Id}' was not found", result.Message);
    }
}