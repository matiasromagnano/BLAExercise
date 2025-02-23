using AutoMapper;
using BLAExercise.API.Models;
using BLAExercise.Data.Models;

namespace BLAExercise.API.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserLoginDto, User>().ReverseMap();
            CreateMap<SneakerDto, Sneaker>().ReverseMap();
            CreateMap<SneakerCreateDto, Sneaker>().ReverseMap();
            CreateMap<SneakerUpdateDto, Sneaker>().ReverseMap();
        }
    }
}
