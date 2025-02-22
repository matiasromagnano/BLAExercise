using BLAExercise.Core.Configuration;
using BLAExercise.Core.Models;
using BLAExercise.Data.Interfaces;
using Microsoft.Extensions.Options;

namespace BLAExercise.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationOptions _applicationOptions;

    public AuthenticationService(IUserRepository userRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _applicationOptions = applicationOptions.Value;
        _userRepository = userRepository;
    }

    public async Task<bool> AuthenticateUser(UserLoginDto user)
    {
        if (user.Email is not null)
        {
            var userFound = await _userRepository.GetUserByEmailAsync(user.Email);
            if (userFound is not null)
            {
                return userFound.Email == user.Email && userFound.Password == user.Password;
            }
        }

        return false;
    }

    public async Task<string> GenerateToken(UserLoginDto user)
    {
        return await Task.FromResult("SomeToken");
    }
}
