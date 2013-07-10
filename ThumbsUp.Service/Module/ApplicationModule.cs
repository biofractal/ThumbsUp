using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;

namespace ThumbsUp.Service.Module
{
	public class ApplicationModule : _BaseModule
	{
		public ApplicationModule(ApplicationService applicationService)	: base("/application")
		{
			Post["/register/new"] = _ =>
			{
				if (Params.AreMissing("Name")) return Params.Missing(Response);
				var application = applicationService.RegisterNew(Params.Name);
				return (application == null) ? ErrorService.Generate(Response, ErrorCode.InvalidParameters) : Response.AsJson(new { ApplicationId = application.Id });
			};

			Post["/register/existing"] = _ =>
			{
				if (Params.AreMissing("Name", "Id")) return Params.Missing(Response);
				var application = applicationService.RegisterExisting(Params.Name, Params.Id);
				return (application == null) ? ErrorService.Generate(Response, ErrorCode.InvalidParameters) : Response.AsJson(new { ApplicationId = application.Id });
			};
		}
	}
}
