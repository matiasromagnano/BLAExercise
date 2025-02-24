using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Application.DTOs;

public class SneakerUpdateDto : BaseUpsertDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be great than 0.")]
    public int Id { get; set; }
}
