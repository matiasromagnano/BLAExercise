using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Services;
using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace BLAExercise.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _authenticationService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOptions<ApplicationOptions>> _optionsMock;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _optionsMock = new Mock<IOptions<ApplicationOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(
            new ApplicationOptions
            {
                JWTSecretKey = "7A2E7Fd7a6F8B11D226EDbfc137A628CC83B76a2A13C62B7bEaEb589D2614E74A68d23E349Cb4B6E2dABDAB475451EDC964DCfF3763E424487B8A6A2246B9B9C"
            });
        _authenticationService = new AuthenticationService(_userRepositoryMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task AuthenticateUser_ValidUser_ReturnsTrue()
    {
        // Arrange
        var user = CustomFaker.User;
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email!))
            .ReturnsAsync(user);

        var userLginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = user.Password
        };

        // Act
        var result = await _authenticationService.AuthenticateUser(userLginDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = CustomFaker.User;
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email!))
            .ReturnsAsync(user);

        var userLginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = user.Password
        };
        // Act
        var token = await _authenticationService.GenerateToken(userLginDto);

        // Assert
        Assert.NotEmpty(token);
    }
}
