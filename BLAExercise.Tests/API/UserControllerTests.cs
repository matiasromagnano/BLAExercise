using AutoMapper;
using BLAExercise.API.Configuration;
using BLAExercise.API.Controllers;
using BLAExercise.API.Models;
using BLAExercise.Core.Exceptions;
using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BLAExercise.Tests.API;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly IMapper _mapper;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        // We are using the actual Mapper to also test that all mappings are correctly configured
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
        _controller = new UserController(_mapper, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsUsers()
    {
        // Arrange
        var users = CustomFaker.Users.Generate(10);
        var usersDto = users.Select(_mapper.Map<UserDto>);
        _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()))
                    .ReturnsAsync(users);

        // Act
        // In a real life scenario we would check for the scenarios combining the parameters for paging and sort.
        // It's implemented and usable but for a matter of time I'll not add the test cases here.
        var result = await _controller.Get();

        // Assert
        _userRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once());
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
        _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Get());

        // Assert
        _userRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once());
        Assert.Equal(result.Message, errorMessage);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(result.Message, errorMessage);
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        // Arrange
        var User = CustomFaker.Users.Generate();
        var UserDto = _mapper.Map<UserDto>(User);
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(User);

        // Act
        var result = await _controller.GetById(User.Id);

        // Assert
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as UserDto;
        Assert.Equivalent(UserDto, response);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        User nullUser = null!;
        var errorMessage = $"User with Id: '{userId}' was not found.";
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(nullUser);

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(userId));

        // Assert
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_IsSuccessful()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        var userLoginDto = _mapper.Map<UserLoginDto>(user);
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Create(userLoginDto);

        // Assert
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once());
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
        var mockUser = CustomFaker.Users.Generate();
        var mockUserDto = _mapper.Map<UserLoginDto>(mockUser);
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                    .ThrowsAsync(new OtherException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Create(mockUserDto));

        // Assert
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest()
    {
        // Arrange
        var mockUser = CustomFaker.Users.Generate();
        var mockUserDto = _mapper.Map<UserLoginDto>(mockUser);
        var errorMessage = "One or more validation errors occurred.";
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                    .ThrowsAsync(new BadRequestException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Create(mockUserDto));

        // Assert
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_IsSuccessful()
    {
        // Arrange
        var mockUser = CustomFaker.Users.Generate();
        var mockUserDto = _mapper.Map<UserDto>(mockUser);
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())
    );

        // Act
        var result = await _controller.Update(mockUserDto);

        // Assert
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }

    [Fact]
    public async Task Update_UserAlreadyExist()
    {
        //Arrange
        var mockUser = CustomFaker.Users.Generate();
        var mockUserDto = _mapper.Map<UserDto>(mockUser);
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                    .ThrowsAsync(new OtherException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Update(mockUserDto));

        // Assert
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Delete_IsSuccessful()
    {
        // Arrange
        var mockUser = CustomFaker.Users.Generate();
        var mockUserDto = _mapper.Map<UserDto>(mockUser);
        _userRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.Delete(mockUser.Id);

        // Assert
        _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }
}
