using AutoMapper;
using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Presentation.Controllers;
using BLAExercise.Presentation.Models;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SneakerCollection.Tests.API
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationService;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _authenticationService = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authenticationService.Object);
        }

        [Fact]
        public async Task GetAuthToken_ReturnsToken()
        {
            // Arrange
            var user = CustomFaker.User;
            var jsonWebToken = "JWTToken";
            var userDto = new UserLoginDto
            {
                Email = user.Email,
                Password = user.Password
            };

            _authenticationService.Setup(s => s.AuthenticateUser(It.IsAny<UserLoginDto>())).ReturnsAsync(true);
            _authenticationService.Setup(s => s.GenerateToken(It.IsAny<UserLoginDto>())).ReturnsAsync(jsonWebToken);

            // Act
            var result = await _controller.GetAuthToken(userDto);

            // Assert
            _authenticationService.Verify(s => s.AuthenticateUser(It.IsAny<UserLoginDto>()), Times.Once());
            _authenticationService.Verify(s => s.GenerateToken(It.IsAny<UserLoginDto>()), Times.Once());
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.Equal(okObjectResult.StatusCode, StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetAuthToken_ReturnsUnauthorized()
        {
            // Arrange
            var user = CustomFaker.User;
            var userDto = new UserLoginDto
            {
                Email = user.Email,
                Password = user.Password
            };

            _authenticationService.Setup(s => s.AuthenticateUser(It.IsAny<UserLoginDto>())).ReturnsAsync(false);

            // Act
            var result = await _controller.GetAuthToken(userDto);

            // Assert
            _authenticationService.Verify(s => s.AuthenticateUser(It.IsAny<UserLoginDto>()), Times.Once());
            _authenticationService.Verify(s => s.GenerateToken(It.IsAny<UserLoginDto>()), Times.Never());
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(unauthorizedResult?.StatusCode, StatusCodes.Status401Unauthorized);
        }
    }
}
