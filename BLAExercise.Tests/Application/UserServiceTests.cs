using AutoMapper;
using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Services;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Tests.Helpers;
using Moq;

namespace BLAExercise.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IMapper _mapper; 
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
        _userService = new UserService(_mockUserRepository.Object, _mapper);
    }

    [Fact]
    public async Task AddAsync_ValidUserLoginDto_ReturnsUserDto()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var user = _mapper.Map<User>(userLoginDto);

        _mockUserRepository.Setup(repo => repo.AddAsync(It.Is<User>(u => u.Email == userLoginDto.Email))).ReturnsAsync(user);

        // Act
        var result = await _userService.AddAsync(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userLoginDto.Email, result.Email);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Email == userLoginDto.Email)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var userLoginDto = CustomFaker.UserLoginDto.Generate();
        var user = _mapper.Map<User>(userLoginDto);

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.AddAsync(userLoginDto));
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesUser()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _mockUserRepository.Setup(repo => repo.DeleteAsync(user.Id)).Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteAsync(user.Id);

        // Assert
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.DeleteAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = CustomFaker.Users.Generate().Id;
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteAsync(id));
        Assert.Equal($"User with Id: '{id}' was not found", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        _mockUserRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var id = CustomFaker.Users.Generate().Id;
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.DeleteAsync(id));
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ValidParameters_ReturnsUserDtos()
    {
        // Arrange
        var users = CustomFaker.Users.Generate(3);
        _mockUserRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(users[0].Email, result[0].Email); // Verify mapping
        _mockUserRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_NoUsersFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ReturnsAsync(new List<User>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetAsync());
        Assert.Equal("No Users were found", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.GetAsync());
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_ValidEmail_ReturnsUserDto()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Id, result.Id); // Verify mapping
        _mockUserRepository.Verify(repo => repo.GetByEmailAsync(user.Email), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var email = CustomFaker.Users.Generate().Email;
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByEmailAsync(email));
        Assert.Equal($"User with Email: '{email}' was not found", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var email = CustomFaker.Users.Generate().Email;
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.GetByEmailAsync(email));
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsUserDto()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email); // Verify mapping
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = CustomFaker.Users.Generate().Id;
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByIdAsync(id));
        Assert.Equal($"User with Id: '{id}' was not found", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var id = CustomFaker.Users.Generate().Id;
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.GetByIdAsync(id));
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidUserDto_ReturnsUpdatedUserDto()
    {
        // Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        var user = CustomFaker.Users.Generate();
        user.Id = userUpdateDto.Id; // Ensure IDs match
        var updatedUser = _mapper.Map<User>(userUpdateDto);

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userUpdateDto.Id)).ReturnsAsync(user);
        _mockUserRepository.Setup(repo => repo.UpdateAsync(It.Is<User>(u => u.Id == userUpdateDto.Id))).ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateAsync(userUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userUpdateDto.Id, result.Id);
        Assert.Equal(userUpdateDto.Email, result.Email); // Verify mapping
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userUpdateDto.Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u.Id == userUpdateDto.Id)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userUpdateDto.Id)).ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateAsync(userUpdateDto));
        Assert.Equal($"User with Id: '{userUpdateDto.Id}' was not found", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userUpdateDto.Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var userUpdateDto = CustomFaker.UserUpdateDto.Generate();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userUpdateDto.Id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _userService.UpdateAsync(userUpdateDto));
        Assert.Equal("Database error", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userUpdateDto.Id), Times.Once);
    }
}