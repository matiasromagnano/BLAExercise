namespace BLAExercise.Domain.Interfaces;

public interface IDomainEntity
{
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }
}