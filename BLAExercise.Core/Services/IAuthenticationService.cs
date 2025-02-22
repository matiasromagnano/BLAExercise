using BLAExercise.Core.Models;

namespace BLAExercise.Core.Services;

public interface IAuthenticationService
{
    Task<bool> AuthenticateUser(UserLoginDto user);
    Task<string> GenerateToken(UserLoginDto user);
}
