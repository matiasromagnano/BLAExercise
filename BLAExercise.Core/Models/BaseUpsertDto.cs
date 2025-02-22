using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Core.Models
{
    public class BaseUpsertDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Brand { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be equal or greater than 0.")]
        public decimal Price { get; set; }
        [Required]
        public float SizeUS { get; set; }
        [Required]
        [Range(1, 9999, ErrorMessage = "Value must between 1 and 9999.")]
        public int Year { get; set; }
        [Range(1, 5, ErrorMessage = "Value must be between 1 and 5.")]
        public int Rate { get; set; }
    }
}
