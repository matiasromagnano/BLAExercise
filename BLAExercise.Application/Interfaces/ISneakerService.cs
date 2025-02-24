using BLAExercise.Application.DTOs;

namespace BLAExercise.Application.Interfaces;

public interface ISneakerService
{
    Task<SneakerDto?> AddAsync(SneakerCreateDto sneakerDto);
    Task DeleteAsync(int id);
    Task<List<SneakerDto>> GetAsync(int page = 1, int pageSize = 10, string sortBy = nameof(SneakerDto.Year), bool descendig = true);
    Task<List<SneakerDto>> GetByUserIdAsync(int userId);
    Task<SneakerDto?> GetByIdAsync(int id);
    Task<List<SneakerDto>> GetByUserEmailAsync(string userEmail);
    Task<SneakerDto?> UpdateAsync(SneakerUpdateDto sneakerDto);
}
