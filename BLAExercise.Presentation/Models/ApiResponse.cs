namespace BLAExercise.Presentation.Models;

public class ApiResponse<T> where T : class
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IDictionary<string, string[]>? Details { get; set; }
}
