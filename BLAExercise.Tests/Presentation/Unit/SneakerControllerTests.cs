using AutoMapper;
using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Presentation.Controllers;
using BLAExercise.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BLAExercise.Tests.Presentation.Unit;

public class SneakerControllerTests
{
    private readonly Mock<ISneakerService> _sneakerServiceMock;
    private readonly SneakerController _controller;

    public SneakerControllerTests()
    {
        _sneakerServiceMock = new Mock<ISneakerService>();
        _controller = new SneakerController(_sneakerServiceMock.Object);
    }

    [Fact]
    public async Task GetSneakers_ReturnsSneakers()
    {
        // Arrange
        var sneakersDto = CustomFaker.SneakersDto.Generate(10);
        _sneakerServiceMock.Setup(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .ReturnsAsync(sneakersDto);

        // Act
        // In a real life scenario we would check for the scenarios combining the parameters for paging and sort.
        // It's implemented and usable but for a matter of time we'll skip those test cases for now.
        var result = await _controller.Get();

        // Assert
        _sneakerServiceMock.Verify(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as IEnumerable<SneakerDto>;
        Assert.NotNull(response);
        Assert.Equivalent(sneakersDto, response);
    }

    [Fact]
    public async Task GetSneakers_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "No Sneakers were found";
        _sneakerServiceMock.Setup(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Get());

        // Assert
        _sneakerServiceMock.Verify(service => service.GetAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        Assert.Equal(result.Message, errorMessage);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(result.Message, errorMessage);
    }

    [Fact]
    public async Task GetSneakerById_ReturnsSneaker()
    {
        // Arrange
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(sneakerDto);

        // Act
        var result = await _controller.GetById(sneakerDto.Id);

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as SneakerDto;
        Assert.Equivalent(sneakerDto, response);
    }

    [Fact]
    public async Task GetSneakerById_ReturnsNotFound()
    {
        // Arrange
        var sneakerId = 1;
        var errorMessage = $"Sneaker with Id: '{sneakerId}' was not found.";
        _sneakerServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(sneakerId));

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetSneakerByUserEmail_ReturnsSneaker()
    {
        // Arrange
        var userEmail = "example@email.com";
        var sneakersDto = CustomFaker.SneakersDto.Generate(10);
        _sneakerServiceMock.Setup(service => service.GetByUserEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(sneakersDto);

        // Act
        var result = await _controller.GetByUserEmail(userEmail);

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByUserEmailAsync(It.IsAny<string>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as IEnumerable<SneakerDto>;
        Assert.Equivalent(sneakersDto, response);
    }

    [Fact]
    public async Task GetSneakerByUserEmail_ReturnsNotFound()
    {
        // Arrange
        var email = "test@email.com";
        var errorMessage = $"Sneakers for Email: '{email}' were not found.";
        _sneakerServiceMock.Setup(service => service.GetByUserEmailAsync(It.IsAny<string>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetByUserEmail(email));

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByUserEmailAsync(It.IsAny<string>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetSneakersByUserId_ReturnsSneakers()
    {
        // Arrange
        var userId = 1;
        var sneakersDto = CustomFaker.SneakersDto.Generate(10);
        _sneakerServiceMock.Setup(service => service.GetByUserIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(sneakersDto);

        // Act
        var result = await _controller.GetByUserId(userId);

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByUserIdAsync(It.IsAny<int>()), Times.Once());
        var okObjectResult = result as OkObjectResult;
        Assert.Equal(StatusCodes.Status200OK, okObjectResult?.StatusCode);
        var response = okObjectResult?.Value as IEnumerable<SneakerDto>;
        Assert.Equivalent(sneakersDto, response);

    }

    [Fact]
    public async Task GetSneakersByUserId_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var errorMessage = $"Sneakers for UserId: '{userId}' were not found.";
        _sneakerServiceMock.Setup(service => service.GetByUserIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetByUserId(userId));

        // Assert
        _sneakerServiceMock.Verify(service => service.GetByUserIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_IsSuccessful()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(service => service.AddAsync(It.IsAny<SneakerCreateDto>()))
            .ReturnsAsync(sneakerDto);

        // Act
        var result = await _controller.Create(sneakerCreateDto);

        // Assert
        _sneakerServiceMock.Verify(service => service.AddAsync(It.IsAny<SneakerCreateDto>()), Times.Once());
        var objectResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status201Created, objectResult?.StatusCode);
        var response = objectResult?.Value as SneakerDto;
        Assert.Equal(sneakerDto.SizeUS, response!.SizeUS);
        Assert.Equal(sneakerDto.Name, response.Name);
        Assert.Equal(sneakerDto.Price, response.Price);
        Assert.Equal(sneakerDto.Brand, response.Brand);
        Assert.Equal(sneakerDto.Rate, response.Rate);
        Assert.Equal(sneakerDto.Year, response.Year);
        Assert.NotEqual(0, response.Id);
    }

    [Fact]
    public async Task Create_SneakerAlreadyExist()
    {
        //Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _sneakerServiceMock.Setup(service => service.AddAsync(It.IsAny<SneakerCreateDto>()))
                    .ThrowsAsync(new CommonException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<CommonException>(async () => await _controller.Create(sneakerCreateDto));

        // Assert
        _sneakerServiceMock.Verify(service => service.AddAsync(It.IsAny<SneakerCreateDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest()
    {
        // Arrange
        var sneakerCreateDto = CustomFaker.SneakerCreateDto.Generate();
        var errorMessage = "One or more validation errors occurred.";
        _sneakerServiceMock.Setup(service => service.AddAsync(It.IsAny<SneakerCreateDto>()))
                    .ThrowsAsync(new BadRequestException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Create(sneakerCreateDto));

        // Assert
        _sneakerServiceMock.Verify(service => service.AddAsync(It.IsAny<SneakerCreateDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_IsSuccessful()
    {
        // Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        var sneakerDto = CustomFaker.SneakersDto.Generate();
        _sneakerServiceMock.Setup(service => service.UpdateAsync(It.IsAny<SneakerUpdateDto>()))
            .ReturnsAsync(sneakerDto);

        // Act
        var result = await _controller.Update(sneakerUpdateDto);

        // Assert
        _sneakerServiceMock.Verify(service => service.UpdateAsync(It.IsAny<SneakerUpdateDto>()), Times.Once());
        var objectResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status200OK, objectResult?.StatusCode);
        var response = objectResult?.Value as SneakerDto;
        Assert.Equal(sneakerDto.SizeUS, response!.SizeUS);
        Assert.Equal(sneakerDto.Name, response.Name);
        Assert.Equal(sneakerDto.Price, response.Price);
        Assert.Equal(sneakerDto.Brand, response.Brand);
        Assert.Equal(sneakerDto.Rate, response.Rate);
        Assert.Equal(sneakerDto.Year, response.Year);
        Assert.NotEqual(0, response.Id);
    }

    [Fact]
    public async Task Update_SneakerAlreadyExist()
    {
        //Arrange
        var sneakerUpdateDto = CustomFaker.SneakerUpdateDto.Generate();
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _sneakerServiceMock.Setup(service => service.UpdateAsync(It.IsAny<SneakerUpdateDto>()))
                    .ThrowsAsync(new CommonException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<CommonException>(async () => await _controller.Update(sneakerUpdateDto));

        // Assert
        _sneakerServiceMock.Verify(service => service.UpdateAsync(It.IsAny<SneakerUpdateDto>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Delete_IsSuccessful()
    {
        // Arrange
        var sneaker = CustomFaker.Sneakers.Generate();
        _sneakerServiceMock.Setup(service => service.DeleteAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.Delete(sneaker.Id);

        // Assert
        _sneakerServiceMock.Verify(service => service.DeleteAsync(It.IsAny<int>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }
}
