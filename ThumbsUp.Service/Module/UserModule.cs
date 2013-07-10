using Nancy;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule(UserService userService) : base("/user")
		{
			Post["/create"] = _ =>
			{
				if (Params.AreMissing("UserName")) return Params.Missing(Response);
				if (!userService.ValidateUserName(Params.UserName)) return ErrorService.Generate(Response, ErrorCode.UserNameTaken);
				var password = userService.CreateUser(Params.UserName, Params.Email);
				return (string.IsNullOrWhiteSpace(password)) ? ErrorService.Generate(Response, ErrorCode.NoUserForCredentials) : Response.AsJson(new { Password = password });
			};

			Post["/get"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return Params.Missing(Response);
				var user = userService.GetUserFromIdentifier(Params.ThumbKey);
				return (user == null) ? ErrorService.Generate(Response, ErrorCode.NoUserForThumbkey) : Response.AsJson(new { User = new { Id = user.Id, UserName = user.UserName, Email = user.Email } });
			};

			Post["/validate"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return Params.Missing(Response);
				var key = userService.ValidateUser(Params.UserName, Params.Password);
				return (key == null) ? ErrorService.Generate(Response, ErrorCode.NoUserForCredentials) : Response.AsJson(new { ThumbKey = key });
			};

			Post["/validate/thumbkey"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return Params.Missing(Response);
				var isValid = userService.ValidateIdentifier(Params.ThumbKey);
				return !isValid ? ErrorService.Generate(Response, ErrorCode.NoUserForThumbkey) : HttpStatusCode.OK;
			};

			Post["/validate/name"] = _ =>
			{
				if (Params.AreMissing("UserName")) return Params.Missing(Response);
				var isValid = userService.ValidateUserName(Params.UserName);
				return !isValid ? ErrorService.Generate(Response, ErrorCode.UserNameTaken) : HttpStatusCode.OK;
			};

			Post["/reset/password"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return Params.Missing(Response);
				var password = userService.ResetPassword(Params.UserName, Params.Password);
				return (password == null) ? ErrorService.Generate(Response, ErrorCode.NoUserForCredentials) : Response.AsJson(new { Password = password });
			};

			Post["/forgot-password/request"] = _ =>
			{
				if (Params.AreMissing("UserName", "Email")) return Params.Missing(Response);
				var token = userService.ForgotPasswordRequest(Params.UserName, Params.Email);
				return (token == null) ? ErrorService.Generate(Response, ErrorCode.NoUserForCredentials) : Response.AsJson(new { Token = token });
			};

			Post["/forgot-password/reset"] = _ =>
			{
				if (Params.AreMissing("UserName", "Token")) return Params.Missing(Response);
				var password = userService.ForgotPasswordReset(Params.UserName, Params.Token);
				return (password == null) ? ErrorService.Generate(Response, ErrorCode.NoUserForCredentials) : Response.AsJson(new { Password = password });
			};

			Post["/logout"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return Params.Missing(Response);
				var success = userService.RemoveUserFromCache(Params.ThumbKey);
				return !success ? ErrorService.Generate(Response, ErrorCode.NoUserForThumbkey) : HttpStatusCode.OK;
			};

		}
	}
}
