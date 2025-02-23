using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BLAExercise.Tests.API;

//public class UserControllerTests
//{
//    private readonly Mock<IMediator> _mediatorMock;
//    private readonly IMapper _mapper;
//    private readonly UserController _controller;
//    private const string Success = nameof(Success);

//    public UserControllerTests()
//    {
//        _mediatorMock = new Mock<IMediator>();
//        //We are using the actual Mapper to also test that all mappings are correctly configured
//        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
//        _mapper = configuration.CreateMapper();
//        _controller = new UserController(_mediatorMock.Object, _mapper);
//    }

//    [Fact]
//    public async Task GetUserById_ReturnsUser()
//    {
//        // Arrange
//        var user = CustomFaker.Users.Generate();
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
//                    .ReturnsAsync(user);

//        // Act
//        var result = await _controller.GetById(user.Id);

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByIdQuery>(), default), Times.Once());
//        var okObjectResult = result.As<OkObjectResult>();
//        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
//        var response = okObjectResult.Value as ApiResponse<User>;
//        response?.StatusCode.Should().Be(StatusCodes.Status200OK);
//        response?.Message.Should().Be(Success);
//        response?.Data.Should().BeEquivalentTo(user);
//        response?.Details.Should().BeNull();
//    }

//    [Fact]
//    public async Task GetUserById_ReturnsNotFound()
//    {
//        // Arrange
//        var userId = 1;
//        var errorMessage = $"User with Id '{userId}' not found";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
//                    .ThrowsAsync(new NotFoundException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(userId));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByIdQuery>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task GetUserById_ReturnsBadRequest()
//    {
//        // Arrange
//        var userId = -1;
//        var errorMessage = "One or more validation errors occurred.";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
//                    .ThrowsAsync(new BadRequestException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.GetById(userId));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByIdQuery>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task GetUserByEmail_ReturnsUser()
//    {
//        // Arrange
//        var user = CustomFaker.Users.Generate();
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default))
//                    .ReturnsAsync(user);

//        // Act
//        var result = await _controller.GetByEmail(user.Email!);

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default), Times.Once());
//        var okObjectResult = result.As<OkObjectResult>();
//        okObjectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
//        var response = okObjectResult.Value as ApiResponse<User>;
//        response?.StatusCode.Should().Be(StatusCodes.Status200OK);
//        response?.Message.Should().Be(Success);
//        response?.Data.Should().BeEquivalentTo(user);
//        response?.Details.Should().BeNull();
//    }

//    [Fact]
//    public async Task GetUserByEmail_ReturnsNotFound()
//    {
//        // Arrange
//        var email = "test@email.com";
//        var errorMessage = $"User with Email '{email}' not found";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default))
//                    .ThrowsAsync(new NotFoundException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetByEmail(email));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task GetUserByEmail_ReturnsBadRequest()
//    {
//        // Arrange
//        var email = "NotValidEmail";
//        var errorMessage = "One or more validation errors occurred.";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default))
//                    .ThrowsAsync(new BadRequestException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.GetByEmail(email));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserByEmailQuery>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Create_IsSuccessful()
//    {
//        // Arrange
//        var user = CustomFaker.Users.Generate();
//        var userDto = _mapper.Map<UserLoginDto>(user);
//        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default))
//                    .ReturnsAsync(user);

//        // Act
//        var result = await _controller.Create(userDto);

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserCommand>(), default), Times.Once());
//        var objectResult = result.As<ObjectResult>();
//        objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
//        var response = objectResult.Value as ApiResponse<User>;
//        response?.StatusCode.Should().Be(StatusCodes.Status200OK);
//        response?.Message.Should().Be(Success);
//        response?.Data.Should().BeEquivalentTo(user);
//        response?.Details.Should().BeNull();
//    }

//    [Fact]
//    public async Task Create_UserAlreadyExist()
//    {
//        //Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserLoginDto>(mockUser);
//        var errorMessage = "Specific error for duplicates thrown by the DB";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default))
//                    .ThrowsAsync(new OtherException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Create(mockUserDto));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserCommand>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Create_ReturnsBadRequest()
//    {
//        // Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserLoginDto>(mockUser);
//        var errorMessage = "One or more validation errors occurred.";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default))
//                    .ThrowsAsync(new BadRequestException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Create(mockUserDto));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateUserCommand>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Update_IsSuccessful()
//    {
//        // Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserDto>(mockUser);
//        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), default));

//        // Act
//        var result = await _controller.Update(mockUserDto);

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), default), Times.Once());
//        var objectResult = result.As<NoContentResult>();
//        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
//    }

//    [Fact]
//    public async Task Update_UserAlreadyExist()
//    {
//        //Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserDto>(mockUser);
//        var errorMessage = "Specific error for duplicates thrown by the DB";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), default))
//                    .ThrowsAsync(new OtherException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Update(mockUserDto));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Update_ReturnsBadRequest()
//    {
//        // Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserDto>(mockUser);
//        var errorMessage = "One or more validation errors occurred.";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), default))
//                    .ThrowsAsync(new BadRequestException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Update(mockUserDto));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), default), Times.Once());

//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Delete_IsSuccessful()
//    {
//        // Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var mockUserDto = _mapper.Map<UserDto>(mockUser);
//        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default));

//        // Act
//        var result = await _controller.Delete(mockUser.Id);

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteUserCommand>(), default), Times.Once());
//        var objectResult = result.As<NoContentResult>();
//        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
//    }

//    [Fact]
//    public async Task Delete_UserAlreadyExist()
//    {
//        //Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var errorMessage = "Specific error for duplicates thrown by the DB";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default))
//                    .ThrowsAsync(new OtherException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<OtherException>(async () => await _controller.Delete(mockUser.Id));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteUserCommand>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
//        result.Data.Count.Should().Be(0);
//    }

//    [Fact]
//    public async Task Delete_ReturnsBadRequest()
//    {
//        // Arrange
//        var mockUser = CustomFaker.Users.Generate();
//        var errorMessage = "One or more validation errors occurred.";
//        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), default))
//                    .ThrowsAsync(new BadRequestException(errorMessage));

//        // Act
//        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _controller.Delete(mockUser.Id));

//        // Assert
//        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteUserCommand>(), default), Times.Once());
//        result.Message.Should().Be(errorMessage);
//        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
//        result.Data.Count.Should().Be(0);
//    }
//}
