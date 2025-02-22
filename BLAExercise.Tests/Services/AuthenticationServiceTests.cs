using BLAExercise.Core.Configuration;
using BLAExercise.Core.Models;
using BLAExercise.Core.Services;
using BLAExercise.Data.Interfaces;
using BLAExercise.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace BLAExercise.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _authService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOptions<ApplicationOptions>> _optionsMock;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _optionsMock = new Mock<IOptions<ApplicationOptions>>();
        _authService = new AuthenticationService(_userRepositoryMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task AuthenticateUser_ValidUser_ReturnsTrue()
    {
        // Arrange
        var user = CustomFaker.User;
        var userLoginDto = new UserLoginDto { Email = user.Email, Password = user.Password };
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(userLoginDto.Email!))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.AuthenticateUser(userLoginDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = CustomFaker.User;
        var userLoginDto = new UserLoginDto { Email = user.Email, Password = user.Password };
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(userLoginDto.Email!))
            .ReturnsAsync(user);

        // Act
        var token = await _authService.GenerateToken(userLoginDto);

        // Assert
        Assert.NotEmpty(token);
    }
}
