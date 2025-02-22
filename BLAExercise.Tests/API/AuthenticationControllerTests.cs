using BLAExercise.Core.Controllers;
using BLAExercise.Core.Models;
using BLAExercise.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SneakerCollection.Tests.API
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IAuthenticationService> _authService;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _authService = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authService.Object);
        }

        [Fact]
        public async Task GetAuthToken_ReturnsToken()
        {
            // Arrange
            var user = new UserLoginDto
            {
                Email = "user@domain.com",
                Password = "RealStrongPassword"
            };
            var jsonWebToken = "JWTToken";
            _authService.Setup(s => s.AuthenticateUser(user)).ReturnsAsync(true);
            _authService.Setup(s => s.GenerateToken(user)).ReturnsAsync(jsonWebToken);

            // Act
            var result = await _controller.GetAuthToken(user);

            // Assert
            _authService.Verify(s => s.AuthenticateUser(It.IsAny<UserLoginDto>()), Times.Once());
            _authService.Verify(s => s.GenerateToken(It.IsAny<UserLoginDto>()), Times.Once());
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.Equal(okObjectResult.StatusCode, StatusCodes.Status200OK);
        }
    }
}
