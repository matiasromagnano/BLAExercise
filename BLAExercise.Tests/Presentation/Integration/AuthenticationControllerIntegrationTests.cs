using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Interfaces;
using BLAExercise.Presentation.Models;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BLAExercise.IntegrationTests.Controllers;

public class AuthenticationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;

    public AuthenticationControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _authenticationServiceMock = new Mock<IAuthenticationService>();

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Override IAuthenticationService with our mock
                services.AddScoped(_ => _authenticationServiceMock.Object);
            });
        }).CreateClient();
    }

    // Helper method to serialize objects to JSON
    private static StringContent ToJsonContent(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task GetAuthToken_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var token = "mock-auth-token";

        _authenticationServiceMock.Setup(s => s.AuthenticateUser(It.IsAny<UserLoginDto>())).ReturnsAsync(true);
        _authenticationServiceMock.Setup(s => s.GenerateToken(It.IsAny<UserLoginDto>())).ReturnsAsync(token);

        // Act
        var response = await _client.PostAsync("/api/Authentication", ToJsonContent(userLoginDto));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        _authenticationServiceMock.Verify(s => s.AuthenticateUser(It.IsAny<UserLoginDto>()), Times.Once);
        _authenticationServiceMock.Verify(s => s.GenerateToken(It.IsAny<UserLoginDto>()), Times.Once);
    }

    [Fact]
    public async Task GetAuthToken_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();

        _authenticationServiceMock.Setup(s => s.AuthenticateUser(It.IsAny<UserLoginDto>())).ReturnsAsync(false);

        // Act
        var response = await _client.PostAsync("/api/Authentication", ToJsonContent(userLoginDto));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        _authenticationServiceMock.Verify(s => s.AuthenticateUser(It.IsAny<UserLoginDto>()), Times.Once);
        _authenticationServiceMock.Verify(s => s.GenerateToken(It.IsAny<UserLoginDto>()), Times.Never);
    }
}