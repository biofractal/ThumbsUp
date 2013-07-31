using Nancy;
using System.Collections.Generic;

namespace ThumbsUp.Service.Domain
{
	public enum ErrorCode 
	{ 
		InternalError,
		InvalidParameters,
		MissingParameters,
		UserNameTaken,
		NoUserForCredentials,
		NoUserForThumbkey,
		NoUserForEmail
	}

	public interface IErrorService
	{
		Response Generate(IResponseFormatter response, ErrorCode code);
		Response MissingParameters(IResponseFormatter response);
		Response InvalidParameters(IResponseFormatter response);
		Response NoUserForCredentials(IResponseFormatter response);
		Response NoUserForThumbkey(IResponseFormatter response);
		Response NoUserForEmail(IResponseFormatter response);
		string Decode(string code);
	}

	public class ErrorService : IErrorService
	{
		private struct Error
		{
			public string Message { get; set; }
			public Nancy.HttpStatusCode StatusCode { get; set; }
		}

		private readonly Dictionary<ErrorCode, Error> Errors = new Dictionary<ErrorCode, Error>{
			{ErrorCode.InternalError, new Error {Message = "Unrecognised internal error", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{ErrorCode.InvalidParameters, new Error {Message = "One or more required values were invalid", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{ErrorCode.MissingParameters, new Error {Message = "One or more required values were missing", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{ErrorCode.UserNameTaken, new Error {Message = "The UserName has already been taken", StatusCode = Nancy.HttpStatusCode.BadRequest}},
			{ErrorCode.NoUserForCredentials, new Error {Message = "No User could be found for the supplied credentials", StatusCode = Nancy.HttpStatusCode.NotFound}},
			{ErrorCode.NoUserForThumbkey, new Error {Message = "No User could be found for the supplied ThumbKey", StatusCode = Nancy.HttpStatusCode.NotFound}},
			{ErrorCode.NoUserForEmail, new Error {Message = "No User could be found for the supplied Email", StatusCode = Nancy.HttpStatusCode.NotFound}}
		};

		public Response MissingParameters(IResponseFormatter response)
		{
			return Generate(response, ErrorCode.MissingParameters);
		}

		public Response InvalidParameters(IResponseFormatter response)
		{
			return Generate(response, ErrorCode.InvalidParameters);
		}

		public Response NoUserForCredentials(IResponseFormatter response)
		{
			return Generate(response, ErrorCode.NoUserForCredentials);
		}

		public Response NoUserForThumbkey(IResponseFormatter response)
		{
			return Generate(response, ErrorCode.NoUserForThumbkey);
		}

		public Response NoUserForEmail(IResponseFormatter response)
		{
			return Generate(response, ErrorCode.NoUserForEmail);
		}

		public Response Generate(IResponseFormatter response, ErrorCode code)
		{
			var error = Errors[code];
			return response.AsJson(new
			{
				ErrorCode = (int)code,
				ErrorMessage = error.Message
			})
			.WithStatusCode(error.StatusCode);
		}

		public string Decode(string candidate)
		{
			int code;
			if (int.TryParse(candidate, out code))
			{ 
				var key = (ErrorCode)code;
				if(Errors.ContainsKey(key)) return Errors[key].Message;
			}
			return Errors[0].Message;
		}
	}
}