using BLAExercise.Application.DTOs;
using BLAExercise.Domain.Models;

namespace BLAExercise.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> AddAsync(UserLoginDto userLoginDto); 
    Task DeleteAsync(int id);
    Task<List<UserDto>> GetAsync(int page = 1, int pageSize = 10, string sortBy = nameof(UserDto.Email), bool descendig = true);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> UpdateAsync(UserUpdateDto userDto);
}
