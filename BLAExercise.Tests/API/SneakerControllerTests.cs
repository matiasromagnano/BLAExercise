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

public class SneakerControllerTests
{
    private readonly Mock<ISneakerRepository> _sneakerRepositoryMock;
    private readonly IMapper _mapper;
    private readonly SneakerController _controller;

    public SneakerControllerTests()
    {
        _sneakerRepositoryMock = new Mock<ISneakerRepository>();
        // We are using the actual Mapper to also test that all mappings are correctly configured
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
        _controller = new SneakerController(_sneakerRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetSneakers_ReturnsSneakers()
    {
        // Arrange
        var sneakers = CustomFaker.Sneakers.Generate(10);
        var sneakersDto = sneakers.Select(_mapper.Map<SneakerDto>);
        _sneakerRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()))
                    .ReturnsAsync(sneakers);

        // Act
        // In a real life scenario we would check for the scenarios combining the parameters for paging and sort.
        // It's implemented and usable but for a matter of time I'll not add the test cases here.
        var result = await _controller.Get();

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once());
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
        _sneakerRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()))
                    .ThrowsAsync(new NotFoundException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Get());

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<GetQueryParameters>()), Times.Once());
        Assert.Equal(result.Message, errorMessage);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(result.Message, errorMessage);
    }

    [Fact]
    public async Task GetSneakerById_ReturnsSneaker()
    {
        // Arrange
        var sneaker = CustomFaker.Sneakers.Generate();
        var sneakerDto = _mapper.Map<SneakerDto>(sneaker);
        _sneakerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(sneaker);

        // Act
        var result = await _controller.GetById(sneaker.Id);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once());
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
        Sneaker nullSneaker = null!;
        var errorMessage = $"Sneaker with Id: '{sneakerId}' was not found.";
        _sneakerRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(nullSneaker);

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(sneakerId));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetSneakerByUserEmail_ReturnsSneaker()
    {
        // Arrange
        var userEmail = "example@email.com";
        var sneakers = CustomFaker.Sneakers.Generate(10);
        var sneakersDto = sneakers.Select(_mapper.Map<SneakerDto>);
        _sneakerRepositoryMock.Setup(repo => repo.GetSneakersByUserEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(sneakers);

        // Act
        var result = await _controller.GetByUserEmail(userEmail);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetSneakersByUserEmailAsync(It.IsAny<string>()), Times.Once());
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
        List<Sneaker> sneakers = new();
        _sneakerRepositoryMock.Setup(repo => repo.GetSneakersByUserEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(sneakers);

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetByUserEmail(email));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetSneakersByUserEmailAsync(It.IsAny<string>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetSneakersByUserId_ReturnsSneakers()
    {
        // Arrange
        var userId = 1;
        var sneakers = CustomFaker.Sneakers.Generate(10);
        var sneakersDto = sneakers.Select(_mapper.Map<SneakerDto>);
        _sneakerRepositoryMock.Setup(repo => repo.GetSneakersByUserIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(sneakers);

        // Act
        var result = await _controller.GetByUserId(userId);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetSneakersByUserIdAsync(It.IsAny<int>()), Times.Once());
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
        List<Sneaker> sneakers = new();
        _sneakerRepositoryMock.Setup(repo => repo.GetSneakersByUserIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(sneakers);

        // Act
        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetByUserId(userId));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.GetSneakersByUserIdAsync(It.IsAny<int>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_IsSuccessful()
    {
        // Arrange
        var sneaker = CustomFaker.Sneakers.Generate();
        var sneakerCreateDto = _mapper.Map<SneakerCreateDto>(sneaker);
        _sneakerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Sneaker>()))
            .ReturnsAsync(sneaker);

        // Act
        var result = await _controller.Create(sneakerCreateDto);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Sneaker>()), Times.Once());
        var objectResult = result as ObjectResult;
        Assert.Equal(StatusCodes.Status201Created, objectResult?.StatusCode);
        var response = objectResult?.Value as SneakerDto;
        Assert.Equal(sneakerCreateDto.SizeUS, response!.SizeUS);
        Assert.Equal(sneakerCreateDto.Name, response.Name);
        Assert.Equal(sneakerCreateDto.Price, response.Price);
        Assert.Equal(sneakerCreateDto.Brand, response.Brand);
        Assert.Equal(sneakerCreateDto.Rate, response.Rate);
        Assert.Equal(sneakerCreateDto.Year, response.Year);
        Assert.NotEqual(0, response.Id);
    }

    [Fact]
    public async Task Create_SneakerAlreadyExist()
    {
        //Arrange
        var mockSneaker = CustomFaker.Sneakers.Generate();
        var mockSneakerDto = _mapper.Map<SneakerCreateDto>(mockSneaker);
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _sneakerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Sneaker>()))
                    .ThrowsAsync(new OtherException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Create(mockSneakerDto));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Sneaker>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest()
    {
        // Arrange
        var mockSneaker = CustomFaker.Sneakers.Generate();
        var mockSneakerDto = _mapper.Map<SneakerCreateDto>(mockSneaker);
        var errorMessage = "One or more validation errors occurred.";
        _sneakerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Sneaker>()))
                    .ThrowsAsync(new BadRequestException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Create(mockSneakerDto));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Sneaker>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_IsSuccessful()
    {
        // Arrange
        var mockSneaker = CustomFaker.Sneakers.Generate();
        var mockSneakerDto = _mapper.Map<SneakerUpdateDto>(mockSneaker);
        _sneakerRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Sneaker>())
);

        // Act
        var result = await _controller.Update(mockSneakerDto);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Sneaker>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }

    [Fact]
    public async Task Update_SneakerAlreadyExist()
    {
        //Arrange
        var mockSneaker = CustomFaker.Sneakers.Generate();
        var mockSneakerDto = _mapper.Map<SneakerUpdateDto>(mockSneaker);
        var errorMessage = "Specific error for duplicates thrown by the DB";
        _sneakerRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Sneaker>()))
                    .ThrowsAsync(new OtherException(errorMessage));

        // Act
        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Update(mockSneakerDto));

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Sneaker>()), Times.Once());
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Delete_IsSuccessful()
    {
        // Arrange
        var mockSneaker = CustomFaker.Sneakers.Generate();
        var mockSneakerDto = _mapper.Map<SneakerDto>(mockSneaker);
        _sneakerRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>()));

        // Act
        var result = await _controller.Delete(mockSneaker.Id);

        // Assert
        _sneakerRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Once());
        var objectResult = result as NoContentResult;
        Assert.Equal(StatusCodes.Status204NoContent, objectResult?.StatusCode);
    }
}
