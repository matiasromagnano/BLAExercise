using Microsoft.AspNetCore.Http;

namespace BLAExercise.Core.Exceptions
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
