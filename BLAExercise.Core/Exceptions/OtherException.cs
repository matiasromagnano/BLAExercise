using Microsoft.AspNetCore.Http;

namespace BLAExercise.Core.Exceptions
{
    public class OtherException : CustomException
    {
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public OtherException(string message)
            : base(message)
        {
        }
    }
}
