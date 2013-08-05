using Nancy;
using Nancy.Helper;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule(IUserService userService, IErrorService error, IPasswordService passwordService, IUserCacheService userCacheService) : base("/user")
		{
			Post["/create"] = _ =>
			{
				if (Params.AreMissing("UserName", "Email")) return error.MissingParameters(Response);
				if (userService.GetUserByName(Params.UserName)!=null) return error.UserNameTaken(Response);
				if (!Params.Email.IsEmail()) return error.InvalidParameters(Response);
				var password = userService.CreateUser(Params.UserName, Params.Email);
				return (string.IsNullOrWhiteSpace(password)) ? error.InvalidParameters(Response) : Response.AsJson(new { Password = password });
			};

			Post["/validate"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return error.MissingParameters(Response);
				var user = userService.GetUserByName(Params.UserName);
				if (user == null || !passwordService.IsPasswordValid(user, Params.Password)) return error.NoUserForCredentials(Response);
				var key = userCacheService.Add(user);
				return (key == null) ? error.InvalidParameters(Response) : Response.AsJson(new { ThumbKey = key });
			};

			Post["/get"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var user = userCacheService.GetUser(Params.ThumbKey);
				return (user == null) ? error.NoUserForThumbkey(Response) : Response.AsJson(new { User = new { Id = user.Id, UserName = user.UserName, Email = user.Email } });
			};

			Post["/validate/thumbkey"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var isValid = userCacheService.Validate(Params.ThumbKey);
				return !isValid ? error.NoUserForThumbkey(Response) : HttpStatusCode.OK;
			};

			Post["/validate/name"] = _ =>
			{
				if (Params.AreMissing("UserName")) return error.MissingParameters(Response);
				var isValid = userService.GetUserByName(Params.UserName)==null;
				return !isValid ? error.UserNameTaken(Response) : HttpStatusCode.OK;
			};

			Post["/reset/password"] = _ =>
			{
				if (Params.AreMissing("UserName", "Password")) return error.MissingParameters(Response);
				var user = userService.GetUserByName(Params.UserName);
				if (user == null || !passwordService.IsPasswordValid(user, Params.Password)) return error.NoUserForCredentials(Response);
				var password = userService.ResetPassword(user);
				return (password == null) ? error.InvalidParameters(Response) : Response.AsJson(new { Password = password });
			};

			Post["/forgot-password/request"] = _ =>
			{
				if (Params.AreMissing("UserName", "Email")) return error.MissingParameters(Response);
				if (!Params.Email.IsEmail()) return error.InvalidParameters(Response);
				var user = userService.GetUserByName(Params.UserName);
				if (user == null) return error.NoUserForCredentials(Response);
				if (user.Email != Params.Email) return error.NoUserForEmail(Response);
				var token = userService.ForgotPasswordRequest(user);
				return (token == null) ? error.InvalidParameters(Response) : Response.AsJson(new { Token = token });
			};

			Post["/forgot-password/reset"] = _ =>
			{
				if (Params.AreMissing("UserName", "Token")) return error.MissingParameters(Response);
				if (!Params.Token.IsGuid()) return error.InvalidParameters(Response);	
				var user = userService.GetUserByName(Params.UserName);
				if (user == null || !passwordService.IsForgotPasswordTokenValid(user, Params.Token)) return error.InvalidForgotPasswordToken(Response);
				var password = userService.ForgotPasswordReset(user);
				return (password == null) ? error.InvalidParameters(Response) : Response.AsJson(new { Password = password });
			};

			Post["/logout"] = _ =>
			{
				if (Params.AreMissing("ThumbKey")) return error.MissingParameters(Response);
				if (!Params.ThumbKey.IsGuid()) return error.InvalidParameters(Response);
				var success = userCacheService.Remove(Params.ThumbKey);
				return !success ? error.NoUserForThumbkey(Response) : HttpStatusCode.OK;
			};

		}
	}
}
