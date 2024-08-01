using Microsoft.AspNetCore.Mvc;

namespace MVCRESTAPI.Controllers.ControllersHelper
{
    public static class ControllerExtensions
    {
        public static IActionResult ApiResponse<T>(this ControllerBase controller, T data, string message = null)
        {
            return controller.Ok(new ApiResponse<T>(data, true, message));
        }

        public static IActionResult ApiErrorResponse<T>(this ControllerBase controller, string message, List<string> errors = null)
        {
            return controller.BadRequest(new ApiResponse<T>(message, false, errors));
        }
    }
}
