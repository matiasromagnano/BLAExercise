using BLAExercise.Application.Configuration;
using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLAExercise.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationOptions _applicationOptions;
    private readonly SymmetricSecurityKey _securityKey;

    public AuthenticationService(IUserRepository userRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _applicationOptions = applicationOptions.Value;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationOptions?.JWTSecretKey ?? throw new ArgumentNullException($"{nameof(_applicationOptions.JWTSecretKey)} is missing from the configuration settings.")));
        _userRepository = userRepository;
    }

    public async Task<bool> AuthenticateUser(UserLoginDto user)
    {
        if (user.Email is not null)
        {
            var userFound = await _userRepository.GetByEmailAsync(user.Email);
            if (userFound is not null)
            {
                return userFound.Email == user.Email && userFound.Password == user.Password;
            }
        }

        return false;
    }

    public async Task<string> GenerateToken(UserLoginDto user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, user.Email!)
        };

        var tokenDetails = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature)
        };

        var jwtHandler = new JwtSecurityTokenHandler();
        var token = await Task.FromResult(jwtHandler.CreateToken(tokenDetails));
        return jwtHandler.WriteToken(token);
    }
}
