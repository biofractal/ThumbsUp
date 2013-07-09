using Nancy;
using System.Collections.Generic;
using System.Net;

namespace ThumbsUp.Service.Domain
{
	public static class ErrorService
	{
		private struct Error
		{
			public string Message { get; set; }
			public Nancy.HttpStatusCode StatusCode { get; set; }
		}

		private static readonly Dictionary<int, Error> Errors = new Dictionary<int, Error>{
			{0, new Error {Message = "Unrecognised internal error", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{1, new Error {Message = "Incorrect UserName or Password", StatusCode = Nancy.HttpStatusCode.Unauthorized}},
			{2, new Error {Message = "Cannot locate a User for the supplied identifier", StatusCode = Nancy.HttpStatusCode.NotFound}},
			{3, new Error {Message = "The UserName has already been taken", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{4, new Error {Message = "The Thumbkey is not valid", StatusCode = Nancy.HttpStatusCode.NotFound}},
			{5, new Error {Message = "The UserName is currently in use", StatusCode = Nancy.HttpStatusCode.Forbidden}},
			{6, new Error {Message = "Incorrect UserName or Email", StatusCode = Nancy.HttpStatusCode.Unauthorized}},
			{7, new Error {Message = "Incorrect UserName or Forgot-Password Token", StatusCode = Nancy.HttpStatusCode.Unauthorized}},
		};

		public static Response Generate(IResponseFormatter response, int code)
		{
			var error = (Errors.ContainsKey(code)) ? Errors[code] : Errors[0];
			return response.AsJson(new
			{
				ErrorCode = code,
				ErrorMessage = error.Message
			})
			.WithStatusCode(error.StatusCode);
		}

		public static string Decode(int code)
		{
			var error = (Errors.ContainsKey(code)) ? Errors[code] : Errors[0];
			return error.Message;
		}
	}
}