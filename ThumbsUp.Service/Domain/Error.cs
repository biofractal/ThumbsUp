using Nancy;
using System.Net;

namespace ThumbsUp.Domain
{
	public class Error
	{
		public static Response Generate(Nancy.IResponseFormatter response, int code)
		{
			switch (code)
			{
				case 1: return GetErrorResponse(response, code, "Incorrect UserName or Password", Nancy.HttpStatusCode.Unauthorized);
				case 2: return GetErrorResponse(response, code, "Cannot locate a User for the supplied identifier", Nancy.HttpStatusCode.NotFound);
				case 3: return GetErrorResponse(response, code, "The UserName has already been taken", Nancy.HttpStatusCode.BadRequest);
				case 4: return GetErrorResponse(response, code, "The Thumbkey is not valid", Nancy.HttpStatusCode.NotFound);
				case 5: return GetErrorResponse(response, code, "The UserName is currently in use", Nancy.HttpStatusCode.Forbidden);
				default: return GetErrorResponse(response, code, "Unrecognised internal error", Nancy.HttpStatusCode.BadRequest);
			}
		}

		private static Response GetErrorResponse(Nancy.IResponseFormatter response, int code, string message, Nancy.HttpStatusCode statusCode)
		{
			return response
				.AsJson(new
				{
					ErrorCode = code,
					ErrorMessage = message
				})
				.WithStatusCode(statusCode);
		}
	}
}