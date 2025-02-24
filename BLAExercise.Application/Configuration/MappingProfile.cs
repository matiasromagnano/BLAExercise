using AutoMapper;
using BLAExercise.Application.DTOs;
using BLAExercise.Domain.Models;

namespace BLAExercise.Application.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserLoginDto, User>().ReverseMap();
            CreateMap<UserUpdateDto, User>().ReverseMap();
            CreateMap<SneakerDto, Sneaker>().ReverseMap();
            CreateMap<SneakerCreateDto, Sneaker>().ReverseMap();
            CreateMap<SneakerUpdateDto, Sneaker>().ReverseMap();
        }
    }
}
