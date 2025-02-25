using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Database;
using BLAExercise.Infrastructure.Repositories;
using BLAExercise.Tests.Helpers;

namespace BLAExercise.Tests.Infrastructure;

/// <summary>
/// Integration tests
/// </summary>
public class GenericRepositoryTests : BaseRepositoryTests
{
    private readonly GenericRepository<User> _genericRepository;

    public GenericRepositoryTests()
    {
        _genericRepository = new GenericRepository<User>(SqlFullConnectionString);
    }

    [Fact]
    public async Task GetByIdAsync_UserExists()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        await _genericRepository.AddAsync(user);

        // Act
        var userFound = await _genericRepository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(userFound);
        Assert.Equivalent(user, userFound);

        // CleanUp
        await _genericRepository.DeleteAsync(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UserDoesNotExists()
    {
        // Arrange & Act
        var userFound = await _genericRepository.GetByIdAsync(int.MinValue);

        // Assert
        Assert.Null(userFound);
    }

    [Fact]
    public async Task AddAsync_EntitySaved()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();

        // Act
        var userAdded = await _genericRepository.AddAsync(user);

        // Assert
        Assert.NotNull(userAdded);
        Assert.Equal(user.Email, userAdded.Email);
        Assert.Equal(user.Password, userAdded.Password);

        // CleanUp
        await _genericRepository.DeleteAsync(userAdded.Id);
    }

    [Fact]
    public async Task DeleteAsync_EntityIsDeleted()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        var userAdded = await _genericRepository.AddAsync(user);

        // Act
        await _genericRepository.DeleteAsync(userAdded!.Id);
        var userDeleted = await _genericRepository.GetByIdAsync(userAdded.Id!);

        // Assert
        Assert.NotNull(userAdded);
        Assert.Null(userDeleted);
    }

    [Fact]
    public async Task UpdateAsync_EntityIsUpdated()
    {
        // Arrange
        var user = CustomFaker.Users.Generate();
        var previousEmail = user.Email;
        var previousPassword = user.Password;
        var userAdded = await _genericRepository.AddAsync(user);
        var email = $"{user.Email}modified";
        var password = $"{user.Password}modified";
        user.Email = email;
        user.Password = password;

        // Act
        await _genericRepository.UpdateAsync(userAdded!);
        var userUpdated = await _genericRepository.GetByIdAsync(userAdded!.Id);

        // Assert
        Assert.NotEqual(userUpdated?.Email, previousEmail);
        Assert.NotEqual(userUpdated?.Email, previousPassword);
        Assert.Equal(userUpdated?.Email, email);
        Assert.Equal(userUpdated?.Password, password);

        // CleanUp
        await _genericRepository.DeleteAsync(userAdded.Id);
    }
}
