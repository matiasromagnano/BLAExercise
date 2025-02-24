using BLAExercise.Application.DTOs;

namespace BLAExercise.Application.Interfaces;

public interface IAuthenticationService
{
    Task<bool> AuthenticateUser(UserLoginDto user);
    Task<string> GenerateToken(UserLoginDto user);
}
