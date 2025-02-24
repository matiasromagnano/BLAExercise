using Microsoft.AspNetCore.Http;

namespace BLAExercise.Application.Exceptions;

public class BadRequestException : CustomException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;

    public BadRequestException(string message)
        : base(message)
    {
    }
}
