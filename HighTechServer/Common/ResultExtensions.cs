using Microsoft.AspNetCore.Mvc;

namespace HighTech.Common
{
    public static class ResultExtensions
    {
        public static JsonResult ResultSuccess<T>(this HttpResponse response, T value, int statusCode = 200)
        {
            response.StatusCode = statusCode;
            return new JsonResult(new Result<T>
            {
                IsSuccess = true,
                Value = value,
                StatusCode = statusCode,
                Error = null,
                ErrorTitle = null
            });
        }

        public static JsonResult ResultFailure<T>(this HttpResponse response, string error, int statusCode = 400, string? errorTitle = null)
        {
            response.StatusCode = statusCode;
            return new JsonResult(new Result<T>
            {
                IsSuccess = false,
                Value = default,
                StatusCode = statusCode,
                Error = error,
                ErrorTitle = errorTitle
            });
        }
    }
}
