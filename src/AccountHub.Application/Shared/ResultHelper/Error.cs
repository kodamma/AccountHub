namespace AccountHub.Application.Shared.ResultHelper
{
    public class Error
    {
        public static readonly Error None = new(string.Empty);
        public static readonly Error NullValue = new("The specified result value is null.");

        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
