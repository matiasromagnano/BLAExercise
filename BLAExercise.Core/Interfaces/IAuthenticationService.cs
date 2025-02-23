using BLAExercise.Data.Models;

namespace BLAExercise.Core.Interfaces;

public interface IAuthenticationService
{
    Task<bool> AuthenticateUser(User user);
    Task<string> GenerateToken(User user);
}
