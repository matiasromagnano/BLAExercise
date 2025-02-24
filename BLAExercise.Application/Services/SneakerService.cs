using AutoMapper;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Exceptions;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;

namespace BLAExercise.Application.Services;

public class SneakerService : ISneakerService
{
    private readonly ISneakerRepository _sneakerRepository;
    private readonly IMapper _mapper;

    public SneakerService(ISneakerRepository sneakerRepository, IMapper mapper)
    {
        _sneakerRepository = sneakerRepository;
        _mapper = mapper;
    }

    public async Task<SneakerDto?> AddAsync(SneakerCreateDto sneakerDto)
    {
        try
        {
            var sneaker = _mapper.Map<Sneaker>(sneakerDto);

            var sneakerAdded = await _sneakerRepository.AddAsync(sneaker);

            return _mapper.Map<SneakerDto>(sneakerAdded);
        }
        catch (Exception ex)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Add {nameof(Sneaker)}");
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var sneaker = await _sneakerRepository.GetByIdAsync(id);

            if (sneaker is null)
            {
                throw new NotFoundException($"{nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{id}' was not found");
            }

            await _sneakerRepository.DeleteAsync(id);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Delete {nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{id}'");
        }
    }

    public async Task<List<SneakerDto>> GetAsync(int page = 1, int pageSize = 10, string sortBy = nameof(Sneaker.Year), bool descendig = true)
    {
        try
        {
            var getQueryParameters = new GetQueryParameters(page, pageSize, sortBy, descendig);

            var sneakers = await _sneakerRepository.GetAsync(getQueryParameters);

            if (sneakers is null || sneakers.Count == 0)
            {
                throw new NotFoundException($"No {nameof(Sneaker)}s were found");
            }

            return sneakers.Select(_mapper.Map<SneakerDto>).ToList();
        }

        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get the {nameof(Sneaker)}s");
        }
    }

    public async Task<SneakerDto?> GetByIdAsync(int id)
    {
        try
        {
            var user = await _sneakerRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"{nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{id}' was not found");
            }

            return _mapper.Map<SneakerDto>(user);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get {nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{id}'");
        }
    }

    public async Task<List<SneakerDto>> GetByUserEmailAsync(string userEmail)
    {
        try
        {
            var sneakers = await _sneakerRepository.GetByUserEmailAsync(userEmail);

            if (sneakers is null || sneakers.Count == 0)
            {
                throw new NotFoundException($"No {nameof(Sneaker)}s found for {nameof(User)} {nameof(User.Email)}: '{userEmail}'");
            }

            return sneakers.Select(_mapper.Map<SneakerDto>).ToList();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get the {nameof(Sneaker)}s for {nameof(User)} {nameof(User.Email)}: '{userEmail}'");
        }
    }

    public async Task<List<SneakerDto>> GetByUserIdAsync(int userId)
    {
        try
        {
            var sneakers = await _sneakerRepository.GetByUserIdAsync(userId);

            if (sneakers is null || sneakers.Count == 0)
            {
                throw new NotFoundException($"No {nameof(Sneaker)}s found for {nameof(Sneaker.UserId)}: '{userId}'");
            }

            return sneakers.Select(_mapper.Map<SneakerDto>).ToList();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Get the {nameof(Sneaker)}s for {nameof(Sneaker.UserId)}: '{userId}'");
        }
    }

    public async Task<SneakerDto?> UpdateAsync(SneakerUpdateDto sneakerDto)
    {
        try
        {
            var sneaker = await _sneakerRepository.GetByIdAsync(sneakerDto.Id);

            if (sneaker is null)
            {
                throw new NotFoundException($"{nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{sneakerDto.Id}' was not found");
            }

            var sneakerToUpdate = _mapper.Map<Sneaker>(sneakerDto);

            var sneakerUpdated = await _sneakerRepository.UpdateAsync(sneakerToUpdate);

            return _mapper.Map<SneakerDto>(sneakerUpdated);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new CommonException(ex.Message ?? $"Something went wrong when trying to Update {nameof(Sneaker)} with {nameof(Sneaker.Id)}: '{sneakerDto.Id}'");
        }
    }
}
