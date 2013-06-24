using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class UserModule : NancyModule
	{
		public UserModule(UserService userService)
			: base("/user")
		{
			Post["/create"] = _ =>
			{
				var username = (string)Request.Form.username;
				if (!userService.ValidateUserName(username)) return ErrorService.Generate(Response, 3);
				var email = (string)Request.Form.email;
				var password = userService.CreateUser(username, email);
				return Response.AsJson(new { Password = password });
			};

			Post["/get"] = _ =>
			{
				var thumbkey = (string)Request.Form.thumbkey;
				var user = userService.GetUserFromIdentifier(thumbkey);
				return (user == null) ? ErrorService.Generate(Response, 2) : Response.AsJson(new { User = new { Id = user.Id, UserName = user.UserName, Email = user.Email} });
			};

			Post["/validate"] = _ =>
			{
				var username = (string)Request.Form.username;
				var password = (string)Request.Form.password;
				var key = userService.ValidateUser(username, password);
				return (key == null) ? ErrorService.Generate(Response, 1) : Response.AsJson( new { ThumbKey = key });
			};

			Post["/validate/thumbkey"] = _ =>
			{
				var thumbkey = (string)Request.Form.thumbkey;
				var isValid = userService.ValidateIdentifier(thumbkey);
				return !isValid ? ErrorService.Generate(Response, 4) : HttpStatusCode.OK;
			};

			Post["/validate/name"] = _ =>
			{
				var username = (string)Request.Form.username;
				var isValid = userService.ValidateUserName(username);
				return !isValid ? ErrorService.Generate(Response, 5) : HttpStatusCode.OK;
			};

			Post["/logout"] = _ =>
			{
				var thumbkey = (string)Request.Form.thumbkey;
				var success = userService.RemoveUserFromCache(thumbkey);
				return !success ? ErrorService.Generate(Response, 4) : HttpStatusCode.OK;
			};

		}
	}
}
