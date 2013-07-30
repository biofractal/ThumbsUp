using Nancy;
using Nancy.Helper;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule(IUserService userService, IErrorService error) : base("/user")
		{
			Post["/create"] = _ =>
			{
				if (Params.AreMissing("UserName", "Email")) return error.MissingParameters(Response);
				if (!userService.IsValidUserName(Params.UserName)) return error.Generate(Response, ErrorCode.UserNameTaken);
				if (!Params.Email.IsEmail()) return error.InvalidParameters(Response);
				var password = userService.CreateUser(Params.UserName, Params.Email);
				return (string.IsNullOrWhiteSpace(password)) ? error.InvalidParameters(Response) : Response.AsJson(new { Password = password });
			};

			Post["/validate"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return error.MissingParameters(Response);
				var key = userService.ValidateUser(Params.UserName, Params.Password);
				return (key == null) ? error.NoUserForCredentials(Response) : Response.AsJson(new { ThumbKey = key });
			};

			Post["/get"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var user = userService.GetUserFromIdentifier(Params.ThumbKey);
				return (user == null) ? error.NoUserForThumbkey(Response) : Response.AsJson(new { User = new { Id = user.Id, UserName = user.UserName, Email = user.Email } });
			};

			Post["/validate/thumbkey"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var isValid = userService.ValidateIdentifier(Params.ThumbKey);
				return !isValid ? error.NoUserForThumbkey(Response) : HttpStatusCode.OK;
			};

			Post["/validate/name"] = _ =>
			{
				if (Params.AreMissing("UserName")) return error.MissingParameters(Response);
				var isValid = userService.IsValidUserName(Params.UserName);
				return !isValid ? error.Generate(Response, ErrorCode.UserNameTaken) : HttpStatusCode.OK;
			};

			Post["/reset/password"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return error.MissingParameters(Response);
				var password = userService.ResetPassword(Params.UserName, Params.Password);
				return (password == null) ? error.NoUserForCredentials(Response) : Response.AsJson(new { Password = password });
			};

			Post["/forgot-password/request"] = _ =>
			{
				if (Params.AreMissing("UserName", "Email")) return error.MissingParameters(Response);
				if (!Params.Email.IsEmail()) return error.InvalidParameters(Response);
				var token = userService.ForgotPasswordRequest(Params.UserName, Params.Email);
				return (token == null) ? error.NoUserForCredentials(Response) : Response.AsJson(new { Token = token });
			};

			Post["/forgot-password/reset"] = _ =>
			{
				if (Params.AreMissing("UserName", "Token")) return error.MissingParameters(Response);
				if (!Params.Token.IsGuid()) return error.InvalidParameters(Response);
				var password = userService.ForgotPasswordReset(Params.UserName, Params.Token);
				return (password == null) ? error.NoUserForCredentials(Response) : Response.AsJson(new { Password = password });
			};

			Post["/logout"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var success = userService.RemoveUserFromCache(Params.ThumbKey);
				return !success ? error.NoUserForThumbkey(Response) : HttpStatusCode.OK;
			};

		}
	}
}
