using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Core.Models;

public class SneakerCreateDto : BaseUpsertDto
{

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = $"Value must be greater than 0.")]
    public int UserId { get; set; }
}
