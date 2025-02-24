using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Application.DTOs;

public class UserLoginDto
{
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// We are treating Password as raw string for simplicity, in a real life case scenario we would have them hashed.
    /// </summary>
    public string? Password { get; set; }
}
