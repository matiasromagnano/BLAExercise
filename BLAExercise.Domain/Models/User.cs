using BLAExercise.Domain.Interfaces;

namespace BLAExercise.Domain.Models;

public class User : IDomainEntity
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime CreationDate { get; set; }
}
