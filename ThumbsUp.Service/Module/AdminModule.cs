using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class AdminModule : NancyModule
	{
		public AdminModule(UserService userService, ApplicationService applicationService) 
		{

			Post["/user/create"] = _ =>
			{
				var username = (string)Request.Form.username;
				var email = (string)Request.Form.email;
				var password = userService.Create(username, email);
				return Response.AsJson(new { Password = password });
			};

			Post["/application/create"] = _ =>
			{
				var name = (string)Request.Form.name;
				var application = applicationService.Create(name);
				return Response.AsJson(new { ApplicationId = application.Id });
			};
		}
	}
}
