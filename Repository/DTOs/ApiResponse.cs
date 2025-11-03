namespace Repository.DTOs
{
    public class APIResponse<T>
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public T? Data { get; set; }

        // success
        public static APIResponse<T> Ok(T data, string message = "Success", string statusCode = "200")
        {
            return new APIResponse<T>
            {
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }

        // fail
        public static APIResponse<T> Fail(string message, string statusCode = "400")
        {
            return new APIResponse<T>
            {
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}
