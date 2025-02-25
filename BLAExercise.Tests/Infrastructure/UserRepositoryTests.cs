using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Infrastructure.Repositories;
using BLAExercise.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLAExercise.Tests.Infrastructure
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _userRepository = new UserRepository(SqlFullConnectionString);
        }

        [Fact]
        public async Task GetByUserEmailAsync_UserExists()
        {
            // Arrange
            var user = CustomFaker.Users.Generate();
            var userAdded = await _userRepository.AddAsync(user);

            // Act
            var userFound = await _userRepository.GetByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(userFound);
            Assert.Equal(user.Email, userFound.Email);

            // CleanUp
            await _userRepository.DeleteAsync(user.Id);
        }

        [Fact]
        public async Task GetByUserIdAsync_UserDoesNotExists()
        {
            // Arrange & Act
            var userFound = await _userRepository.GetByEmailAsync("some@email.com");

            // Assert
            Assert.Null(userFound);
        }
    }
}
