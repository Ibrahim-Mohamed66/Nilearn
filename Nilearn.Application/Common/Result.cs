namespace Nilearn.Application.Common
{
    public class Result<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // Success response
        public static Result<T> SuccessResponse(T data, string message = "")
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        // Failure response
        public static Result<T> FailureResponse(List<string> errors, string message = "")
        {
            return new Result<T>
            {
                Success = false,
                Errors = errors,
                Message = message
            };
        }
    }
}
