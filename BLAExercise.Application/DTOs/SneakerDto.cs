namespace BLAExercise.Application.DTOs;

public class SneakerDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public float SizeUS { get; set; }
    public int Year { get; set; }
    public int Rate { get; set; }
    public DateTime CreationDate { get; set; }
    public int UserId { get; set; }
}
