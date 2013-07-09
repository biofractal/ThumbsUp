using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class ApplicationModule : NancyModule
	{
		public ApplicationModule(ApplicationService applicationService)	: base("/application")
		{
			Post["/register/new"] = _ =>
			{
				var name = (string)Request.Form.name;
				var application = applicationService.RegisterNew(name);
				return Response.AsJson(new { ApplicationId = application.Id }).WithStatusCode(HttpStatusCode.OK);
			};

			Post["/register/existing"] = _ =>
			{
				var name = (string)Request.Form.name;
				var id = (string)Request.Form.id;
				var application = applicationService.RegisterExisting(name, id);
				return Response.AsJson(new { ApplicationId = application.Id }).WithStatusCode(HttpStatusCode.OK);
			};
		}
	}
}
