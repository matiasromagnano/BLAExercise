using Microsoft.AspNetCore.Http;

namespace BLAExercise.Application.Exceptions
{
    public class NotFoundException : CustomException
    {
        public override int StatusCode => StatusCodes.Status404NotFound;

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
