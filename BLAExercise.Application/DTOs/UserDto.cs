using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Application.DTOs;

public class UserDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be great than 0.")]
    public int Id { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime CreationDate { get; set; }
}
