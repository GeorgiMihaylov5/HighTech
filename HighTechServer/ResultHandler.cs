using Microsoft.AspNetCore.Mvc;

namespace HighTechServer
{
	public static class ResultHandler
	{
		public static JsonResult Success<T>(this HttpResponse response, T data, int statusCode = 200)
		{
			response.StatusCode = statusCode;
			return new JsonResult(new
			{
				StatusCode = statusCode,
				Data = data
			});
		}

		public static JsonResult Error(this HttpResponse response, string? errorMessage, ErrorCode errorCode, int statusCode)
		{
			response.StatusCode = statusCode;

			return new JsonResult(new
			{
				Message = errorMessage,
				ErrorCode = errorCode.ToString(),
				StatusCode = statusCode,
				FaultCode = "CONTROLLER_EXCEPTION",
			});
		}
	}
}
