using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class SecurityModule : NancyModule
	{
		public SecurityModule(UserService userService, ApplicationService applicationService)
		{
			Post["/login"] = _ =>
			{
				var username = (string)Request.Form.username;
				var password = (string)Request.Form.password;
				var result = userService.GetUserAndKey(username, password);
				if (result == null) return HttpStatusCode.Unauthorized;
				return Response.AsJson(new { ThumbKey = result.Item1, User = result.Item2 });
			};

			Post["/check"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				var isValid = userService.Check(identifier);
				return Response.AsJson(new { IsValid = isValid});
			};

			Post["/logout"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				userService.Remove(identifier);
				return HttpStatusCode.OK;
			};

		}
	}
}
