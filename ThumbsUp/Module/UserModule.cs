using Nancy;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class UserModule : NancyModule
	{
		public UserModule(UserService userService, ApplicationService applicationService):base("/user")
		{
			Post["/authenticate"] = _ =>
			{
				var username = (string)Request.Form.username;
				var password = (string)Request.Form.password;
				var result = userService.GetUserAndKey(username, password);
				if (result == null) return HttpStatusCode.Unauthorized;
				return Response.AsJson(new {ThumbKey = result.Item1, User=result.Item2});
			};

			Post["/validate"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				var isValid = userService.Validate(identifier);
				return Response.AsJson(new { IsValid = isValid });
			};

			Post["/remove"] = _ =>
			{
				var identifier = (string)Request.Form.identifier;
				userService.Remove(identifier);
				return HttpStatusCode.OK;
			};

			Post["/create"] = _ =>
			{
				var username = (string)Request.Form.username;
				var email = (string)Request.Form.email;
				var password = userService.Create(username, email);
				return Response.AsJson(new { Password = password });
			};
		}
	}
}
