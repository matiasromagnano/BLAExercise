namespace BLAExercise.Core.Exceptions
{
    public abstract class CustomException : Exception
    {

        public virtual int StatusCode { get; set; }

        public CustomException(string message)
            : base(message)
        {
        }
    }
}
