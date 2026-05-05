namespace Nilearn.Application.Common
{
    public class Result<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static Result<T> SuccessResponse(T data = null, string message = "")
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
    }
}
