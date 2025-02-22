using BLAExercise.Data.Models;
using BLAExercise.Data.Repositories;
using BLAExercise.Tests.Helpers;

namespace BLAExercise.Tests.Data;

public class GenericRepositoryTests
{
    private readonly GenericRepository<User> _genericRepository;

    public GenericRepositoryTests()
    {
        _genericRepository = new GenericRepository<User>("SqlConnectionString");
    }

    [Fact]
    public async Task GetByIdAsync_UserExists()
    {
        // Arrange
        var user = CustomFaker.User;

        // Act
        var result = await _genericRepository.GetByIdAsync(user.Id);

        // Assert
        // TODO: Future 
    }

    [Fact]
    public async Task GetByIdAsync_UserDoesNotExists()
    {
        // Arrange
        // SetUp done in TestFixture 

        // Act
        var result = await _genericRepository.GetByIdAsync(int.MinValue);

        // Assert
        // TODO: Future 
    }

    [Fact]
    public async Task AddAsync_EntitySaved()
    {
        // Arrange
        var user = CustomFaker.User;

        // Act & Assert
        await _genericRepository.AddAsync(user);

    }

    [Fact]
    public async Task DeleteAsync_EntityIsDeleted()
    {
        // Arrange
        var user = CustomFaker.User;

        // Act
        await _genericRepository.DeleteAsync(user.Id);
        var result = await _genericRepository.GetByIdAsync(user.Id);

        // Assert
        // TODO: Future 
    }

    [Fact]
    public async Task UpdateAsync_EntityIsUpdated()
    {
        // Arrange
        var user = CustomFaker.User;
        var newEmail = "modified@email.com";
        var newPassword = "ModifiedPassword";

        // Act
        user.Email = newEmail;
        user.Password = newPassword;

        await _genericRepository.UpdateAsync(user);
        var result = await _genericRepository.GetByIdAsync(user.Id);

        // Assert
        // TODO: Future
    }
}
