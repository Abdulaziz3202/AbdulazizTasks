namespace MVCRESTAPI.Controllers.ControllersHelper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }

        public ApiResponse(T data, bool success = true, string message = null)
        {
            Data = data;
            Success = success;
            Message = message;
            Errors = new List<string>();
        }

        public ApiResponse(string message, bool success = false, List<string> errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors ?? new List<string>();
        }
    }

}
