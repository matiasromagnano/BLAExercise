using AutoMapper;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;

namespace BLAExercise.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> AddAsync(UserLoginDto userLoginDto)
    {
        try
        {
            var user = _mapper.Map<User>(userLoginDto);

            var userAdded = await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(userAdded);
        }
        catch (Exception ex)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Add {nameof(User)} with {nameof(UserLoginDto.Email)}: '{userLoginDto.Email}'");
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"User with {nameof(UserDto.Id)}: '{id}' was not found");
            }

            await _userRepository.DeleteAsync(id);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Delete {nameof(User)} with {nameof(User.Id)}: '{id}'");
        }
    }

    public async Task<List<UserDto>> GetAsync(int page = 1, int pageSize = 10, string sortBy = nameof(User.Email), bool descendig = true)
    {
        try
        {
            var getQueryParameters = new GetQueryParameters(page, pageSize, sortBy, descendig);

            var users = await _userRepository.GetAsync(getQueryParameters);

            if (users is null || users.Count == 0)
            {
                throw new NotFoundException($"No {nameof(User)}s were found");
            }

            return users.Select(_mapper.Map<UserDto>).ToList();
        }

        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get the {nameof(User)}s");
        }
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
            {
                throw new NotFoundException($"User with {nameof(UserDto.Email)}: '{email}' was not found");
            }

            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get {nameof(User)} with {nameof(UserDto.Email)}: '{email}'");
        }
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"{nameof(User)} with {nameof(UserDto.Id)}: '{id}' was not found");
            }

            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get {nameof(User)} with {nameof(UserDto.Id)}: '{id}'");
        }
    }

    public async Task<UserDto?> UpdateAsync(UserUpdateDto userDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userDto.Id);

            if (user is null)
            {
                throw new NotFoundException($"{nameof(User)} with {nameof(UserDto.Id)}: '{userDto.Id}' was not found");
            }
            
            var userToUpdate = _mapper.Map<User>(userDto);   

            var userUpdated = await _userRepository.UpdateAsync(userToUpdate);

            return _mapper.Map<UserDto>(userUpdated);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Update {nameof(User)} with {nameof(UserLoginDto.Email)}: '{userDto.Email}'");
        }
    }
}
