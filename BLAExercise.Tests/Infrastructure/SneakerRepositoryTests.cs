using BLAExercise.Domain.Models;
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
    public class SneakerRepositoryTests : BaseRepositoryTests
    {
        private readonly SneakerRepository _sneakerRepository;
        private readonly UserRepository _userRepository;

        public SneakerRepositoryTests()
        {
            _sneakerRepository = new SneakerRepository(SqlFullConnectionString);
            _userRepository = new UserRepository(SqlFullConnectionString);
        }

        [Fact]
        public async Task GetByUserIdAsync_SneakersExist()
        {
            // Arrange
            var user = CustomFaker.Users.Generate();
            var userAdded = await _userRepository.AddAsync(user);

            var sneaker = CustomFaker.Sneakers.Generate();
            sneaker.UserId = userAdded.Id;

            await _sneakerRepository.AddAsync(sneaker);

            // Act
            var sneakersFound = await _sneakerRepository.GetByUserIdAsync(sneaker.UserId);

            // Assert
            Assert.NotNull(sneakersFound);
            Assert.Single(sneakersFound);

            // CleanUp
            await _sneakerRepository.DeleteAsync(sneaker.Id);
            await _userRepository.DeleteAsync(user.Id);
        }

        [Fact]
        public async Task GetByUserEmailAsync_NoSneakersExist()
        {
            // Arrange & Act
            var sneakersFound = await _sneakerRepository.GetByUserIdAsync(int.MinValue);

            // Assert
            Assert.Empty(sneakersFound);
        }

        [Fact]
        public async Task GetByUserEmailAsync_SneakersExist()
        {
            // Arrange
            var user = CustomFaker.Users.Generate();
            var userAdded = await _userRepository.AddAsync(user);

            var sneaker = CustomFaker.Sneakers.Generate();
            sneaker.UserId = userAdded.Id;

            await _sneakerRepository.AddAsync(sneaker);

            // Act
            var sneakersFound = await _sneakerRepository.GetByUserEmailAsync(user.Email);

            // Assert
            Assert.NotNull(sneakersFound);
            Assert.Single(sneakersFound);

            // CleanUp
            await _sneakerRepository.DeleteAsync(sneaker.Id);
            await _userRepository.DeleteAsync(user.Id);
        }

        [Fact]
        public async Task GetByUserIdAsync_UserDoesNotExists()
        {
            // Arrange & Act
            var sneakersFound = await _sneakerRepository.GetByUserEmailAsync("some@email.com");

            // Assert
            Assert.Empty(sneakersFound);
        }
    }
}
