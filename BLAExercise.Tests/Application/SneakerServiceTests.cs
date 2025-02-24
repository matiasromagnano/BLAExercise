﻿using AutoMapper;
using BLAExercise.Application.Configuration;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Services;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Tests.Helpers;
using Moq;

namespace BLAExercise.Application.Tests.Services;

public class SneakerServiceTests
{
    private readonly Mock<ISneakerRepository> _mockSneakerRepository;
    private readonly IMapper _mapper;
    private readonly SneakerService _sneakerService;

    public SneakerServiceTests()
    {
        _mockSneakerRepository = new Mock<ISneakerRepository>();
        // We are using the actual Mapper to also test that all mappings are correctly configured
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
        _sneakerService = new SneakerService(_mockSneakerRepository.Object, _mapper);
    }

    [Fact]
    public async Task AddAsync_ValidSneakerDto_ReturnsSneakerDto()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var sneaker = CustomFaker.Sneakers.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _mockSneakerRepository.Setup(repo => repo.AddAsync(It.IsAny<Sneaker>())).ReturnsAsync(sneaker);

        // Act
        var result = await _sneakerService.AddAsync(sneakerCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sneaker.Id, result.Id);
        Assert.Equal(sneaker.Rate, result.Rate);
        Assert.Equal(sneaker.Year, result.Year);
        Assert.Equal(sneaker.Price, result.Price);
        Assert.Equal(sneaker.SizeUS, result.SizeUS);
        _mockSneakerRepository.Verify(repo => repo.AddAsync(It.IsAny<Sneaker>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var sneaker = CustomFaker.Sneakers.Generate();
        _mockSneakerRepository.Setup(repo => repo.AddAsync(It.IsAny<Sneaker>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.AddAsync(sneakerCreateDto));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.AddAsync(It.IsAny<Sneaker>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesSneaker()
    {
        // Arrange
        var sneaker = CustomFaker.Sneakers.Generate();
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(sneaker.Id)).ReturnsAsync(sneaker);
        _mockSneakerRepository.Setup(repo => repo.DeleteAsync(sneaker.Id)).Returns(Task.CompletedTask);

        // Act
        await _sneakerService.DeleteAsync(sneaker.Id);

        // Assert
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(sneaker.Id), Times.Once);
        _mockSneakerRepository.Verify(repo => repo.DeleteAsync(sneaker.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SneakerNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = CustomFaker.Sneakers.Generate().Id;
        Sneaker sneaker = null;
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(sneaker);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.DeleteAsync(id));
        Assert.Equal($"Sneaker with Id: '{id}' was not found", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        _mockSneakerRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var id = CustomFaker.Sneakers.Generate().Id;
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.DeleteAsync(id));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ValidParameters_ReturnsSneakerDtos()
    {
        // Arrange
        var sneakers = CustomFaker.Sneakers.Generate(3);
        var sneakerDtos = CustomFaker.SneakersDto.Generate(3);
        _mockSneakerRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ReturnsAsync(sneakers);

        // Act
        var result = await _sneakerService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        _mockSneakerRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_NoSneakersFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockSneakerRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ReturnsAsync(new List<Sneaker>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.GetAsync());
        Assert.Equal("No Sneakers were found", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        _mockSneakerRepository.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.GetAsync());
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsSneakerDto()
    {
        // Arrange
        var sneaker = CustomFaker.Sneakers.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(sneaker.Id)).ReturnsAsync(sneaker);

        // Act
        var result = await _sneakerService.GetByIdAsync(sneaker.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sneaker.Id, result.Id);
        Assert.Equal(sneaker.Rate, result.Rate);
        Assert.Equal(sneaker.Year, result.Year);
        Assert.Equal(sneaker.Price, result.Price);
        Assert.Equal(sneaker.SizeUS, result.SizeUS);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(sneaker.Id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_SneakerNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = CustomFaker.Sneakers.Generate().Id;
        Sneaker sneaker = null;
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(sneaker);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.GetByIdAsync(id));
        Assert.Equal($"Sneaker with Id: '{id}' was not found", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var id = CustomFaker.Sneakers.Generate().Id;
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.GetByIdAsync(id));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ValidEmail_ReturnsSneakerDtos()
    {
        // Arrange
        var userEmail = CustomFaker.User.Email;
        var sneakers = CustomFaker.Sneakers.Generate(2);
        var sneakerDtos = CustomFaker.SneakersDto.Generate(2);
        _mockSneakerRepository.Setup(repo => repo.GetByUserEmailAsync(userEmail)).ReturnsAsync(sneakers);

        // Act
        var result = await _sneakerService.GetByUserEmailAsync(userEmail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockSneakerRepository.Verify(repo => repo.GetByUserEmailAsync(userEmail), Times.Once);
    }

    [Fact]
    public async Task GetByUserEmailAsync_NoSneakersFound_ThrowsNotFoundException()
    {
        // Arrange
        var userEmail = CustomFaker.User.Email;
        _mockSneakerRepository.Setup(repo => repo.GetByUserEmailAsync(userEmail)).ReturnsAsync(new List<Sneaker>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.GetByUserEmailAsync(userEmail));
        Assert.Equal($"No Sneakers found for User Email: '{userEmail}'", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByUserEmailAsync(userEmail), Times.Once);
    }

    [Fact]
    public async Task GetByUserEmailAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var userEmail = CustomFaker.User.Email;
        _mockSneakerRepository.Setup(repo => repo.GetByUserEmailAsync(userEmail)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.GetByUserEmailAsync(userEmail));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByUserEmailAsync(userEmail), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_ValidUserId_ReturnsSneakerDtos()
    {
        // Arrange
        var userId = CustomFaker.User.Id;
        var sneakers = CustomFaker.Sneakers.Generate(2);
        var sneakerDtos = CustomFaker.SneakersDto.Generate(2);
        _mockSneakerRepository.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(sneakers);

        // Act
        var result = await _sneakerService.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockSneakerRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_NoSneakersFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = CustomFaker.User.Id;
        _mockSneakerRepository.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(new List<Sneaker>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.GetByUserIdAsync(userId));
        Assert.Equal($"No Sneakers found for UserId: '{userId}'", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var userId = CustomFaker.User.Id;
        _mockSneakerRepository.Setup(repo => repo.GetByUserIdAsync(userId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.GetByUserIdAsync(userId));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidSneakerDto_ReturnsUpdatedSneakerDto()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        var sneaker = CustomFaker.Sneakers.Generate();
        var updatedSneaker = CustomFaker.Sneakers.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(sneakerUpdateDto.Id)).ReturnsAsync(sneaker);
        _mockSneakerRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Sneaker>())).ReturnsAsync(updatedSneaker);

        // Act
        var result = await _sneakerService.UpdateAsync(sneakerUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedSneaker.Id, result.Id);
        Assert.Equal(updatedSneaker.Rate, result.Rate);
        Assert.Equal(updatedSneaker.Year, result.Year);
        Assert.Equal(updatedSneaker.Price, result.Price);
        Assert.Equal(updatedSneaker.SizeUS, result.SizeUS);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(sneakerUpdateDto.Id), Times.Once);
        _mockSneakerRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Sneaker>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SneakerNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(sneakerUpdateDto.Id)).ReturnsAsync((Sneaker)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sneakerService.UpdateAsync(sneakerUpdateDto));
        Assert.Equal($"Sneaker with Id: '{sneakerUpdateDto.Id}' was not found", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(sneakerUpdateDto.Id), Times.Once);
        _mockSneakerRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Sneaker>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RepositoryThrowsException_ThrowsCommonException()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        _mockSneakerRepository.Setup(repo => repo.GetByIdAsync(sneakerUpdateDto.Id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CommonException>(() => _sneakerService.UpdateAsync(sneakerUpdateDto));
        Assert.Equal("Database error", exception.Message);
        _mockSneakerRepository.Verify(repo => repo.GetByIdAsync(sneakerUpdateDto.Id), Times.Once);
    }
}