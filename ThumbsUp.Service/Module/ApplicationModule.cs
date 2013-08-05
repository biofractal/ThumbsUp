using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;

namespace ThumbsUp.Service.Module
{
	public class ApplicationModule : _BaseModule
	{
		public ApplicationModule(IApplicationService applicationService, IErrorService error) : base("/application")
		{
			Post["/register/new"] = _ =>
			{
				if (Params.AreMissing("Name")) return error.MissingParameters(Response);
				var application = applicationService.RegisterNew(Params.Name);
				return (application == null) ? error.InvalidParameters(Response) : Response.AsJson(new { ApplicationId = application.Id });
			};

			Post["/register/existing"] = _ =>
			{
				if (Params.AreMissing("Name", "Id")) return error.MissingParameters(Response);
				if (!Params.Id.IsGuid()) return error.InvalidParameters(Response);
				var application = applicationService.RegisterExisting(Params.Name, Params.Id);
				return (application == null) ? error.InvalidParameters(Response) : Response.AsJson(new { ApplicationId = application.Id });
			};
		}
	}
}
