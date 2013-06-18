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
			Post["/validate-user"] = _ =>
			{
				var username = (string)Request.Form.username;
				var password = (string)Request.Form.password;
				var result = userService.ValidateUser(username, password);
				if (result==null) return HttpStatusCode.Unauthorized;
				return Response.AsJson((object)result);
			};

			Post["/get-user-from-identifier"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				var result = userService.GetUserFromIdentifier(identifier);
				if (result == null) return HttpStatusCode.Unauthorized;
				return Response.AsJson((object)result);
			};

			Post["/logout"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				var success = userService.Remove(identifier);
				return (success)? HttpStatusCode.OK : HttpStatusCode.NotFound;
			};

		}
	}
}
