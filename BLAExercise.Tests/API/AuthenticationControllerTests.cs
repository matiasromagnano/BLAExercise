using AutoMapper;
using BLAExercise.API.Configuration;
using BLAExercise.API.Models;
using BLAExercise.Core.Controllers;
using BLAExercise.Core.Interfaces;
using BLAExercise.Data.Models;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SneakerCollection.Tests.API
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationService;
        private readonly IMapper _mapper;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _authenticationService = new Mock<IAuthenticationService>();
            // We are using the actual Mapper to also test that all mappings are correctly configured
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = configuration.CreateMapper();

            _controller = new AuthenticationController(_authenticationService.Object, _mapper);
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

            _authenticationService.Setup(s => s.AuthenticateUser(It.IsAny<User>())).ReturnsAsync(true);
            _authenticationService.Setup(s => s.GenerateToken(It.IsAny<User>())).ReturnsAsync(jsonWebToken);

            // Act
            var result = await _controller.GetAuthToken(userDto);

            // Assert
            _authenticationService.Verify(s => s.AuthenticateUser(It.IsAny<User>()), Times.Once());
            _authenticationService.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Once());
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

            _authenticationService.Setup(s => s.AuthenticateUser(It.IsAny<User>())).ReturnsAsync(false);

            // Act
            var result = await _controller.GetAuthToken(userDto);

            // Assert
            _authenticationService.Verify(s => s.AuthenticateUser(It.IsAny<User>()), Times.Once());
            _authenticationService.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Never());
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(unauthorizedResult?.StatusCode, StatusCodes.Status401Unauthorized);
        }
    }
}
