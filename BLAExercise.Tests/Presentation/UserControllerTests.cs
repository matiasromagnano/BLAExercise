using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Interfaces;
using BLAExercise.Presentation.Controllers;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BLAExercise.Tests.API;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsUsers()
    {
        // Arrange
        var usersDto = CustomFaker.UsersDto.Generate(10);
        _userServiceMock.Setup(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .ReturnsAsync(usersDto);

        // Act
        // In a real life scenario we would check for the scenarios combining the parameters for paging and sort.
        // It's implemented and usable but for a matter of time I'll not add the test cases here.
        var result = await _controller.Get();

        // Assert
        _userServiceMock.Verify(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as IEnumerable<UserDto>;
        Assert.NotNull(response);
        Assert.Equivalent(usersDto, response);
    }

    [Fact]
    public async Task GetUsers_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "No Users were found";
        _userServiceMock.Setup(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Get());

        // Assert
        _userServiceMock.Verify(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        Assert.Equal(result.Message, errorMessage);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(result.Message, errorMessage);
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        // Arrange
        var userDto = CustomFaker.UsersDto.Generate();
        _userServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(userDto);

        // Act
        var result = await _controller.GetById(userDto.Id);

        // Assert
        _userServiceMock.Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as UserDto;
        Assert.Equivalent(userDto, response);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var errorMessage = $"User with Id: '{userId}' was not found.";
        _userServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(userId));

        // Assert
        _userServiceMock.Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_IsSuccessful()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var usersDto = CustomFaker.UsersDto.Generate();
        usersDto.Email = userLoginDto.Email;
        usersDto.Password = userLoginDto.Password;

        _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserLoginDto>()))
            .ReturnsAsync(usersDto);

        // Act
        var result = await _controller.Create(userLoginDto);

        // Assert
        _userServiceMock.Verify(service => service.AddAsync(It.IsAny<UserLoginDto>()), Times.Once());
        var objectResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status201Created, objectResult?.StatusCode);
        var response = objectResult?.Value as UserDto;
        Assert.Equal(userLoginDto.Email, response!.Email);
        Assert.Equal(userLoginDto.Password, response.Password);
        Assert.NotEqual(0, response.Id);
    }

    [Fact]
    public async Task Create_UserAlreadyExist()
    {
        //Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserLoginDto>()))
                    .ThrowsAsync(new CommonException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<CommonException>(async () => await _controller.Create(userLoginDto));

        // Assert
        _userServiceMock.Verify(service => service.AddAsync(It.IsAny<UserLoginDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var errorMessage = "One or more validation errors occurred.";
        _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserLoginDto>()))
                    .ThrowsAsync(new BadRequestException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Create(userLoginDto));

        // Assert
        _userServiceMock.Verify(service => service.AddAsync(It.IsAny<UserLoginDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_IsSuccessful()
    {
        // Arrange
        var userDto = CustomFaker.UsersDto.Generate();
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        _userServiceMock.Setup(service => service.UpdateAsync(It.IsAny<UserUpdateDto>()))
            .ReturnsAsync(userDto);

        // Act
        var result = await _controller.Update(userUpdateDto);

        // Assert
        _userServiceMock.Verify(service => service.UpdateAsync(It.IsAny<UserUpdateDto>()), Times.Once());
        var objectResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status200OK, objectResult?.StatusCode);
        var response = objectResult?.Value as UserDto;
        Assert.Equal(userDto.Email, response!.Email);
        Assert.Equal(userDto.Password, response.Password);
        Assert.NotEqual(0, response.Id);
    }

    [Fact]
    public async Task Update_UserAlreadyExist()
    {
        //Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _userServiceMock.Setup(service => service.UpdateAsync(It.IsAny<UserUpdateDto>()))
                    .ThrowsAsync(new CommonException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<CommonException>(async () => await _controller.Update(userUpdateDto));

        // Assert
        _userServiceMock.Verify(service => service.UpdateAsync(It.IsAny<UserUpdateDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Delete_IsSuccessful()
    {
        // Arrange
        var mockUser = CustomFaker.Users.Generate();
        _userServiceMock.Setup(service => service.DeleteAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.Delete(mockUser.Id);

        // Assert
        _userServiceMock.Verify(service => service.DeleteAsync(It.IsAny<int>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }
}
