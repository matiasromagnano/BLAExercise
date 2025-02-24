using Microsoft.AspNetCore.Http;

namespace BLAExercise.Application.Exceptions
{
    public class CommonException : CustomException
    {
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public CommonException(string message)
            : base(message)
        {
        }
    }
}
