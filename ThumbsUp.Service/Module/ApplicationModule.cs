using Nancy;
using Nancy.Helper;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;

namespace ThumbsUp.Service.Module
{
	public class ApplicationModule : _BaseModule
	{
		public ApplicationModule(IApplicationService applicationService, IErrorService error)
			: base("/application")
		{
			Post["/register"] = _ =>
			{
				if (Params.AreMissing("SingleUseToken", "Name")) return error.MissingParameters(Response);
				if (!Params.SingleUseToken.IsGuid()) return error.InvalidParameters(Response);
				if (!applicationService.AuthoriseSingleUseToken(Params.SingleUseToken)) return error.PermissionDenied(Response);

				var application = applicationService.Register(Params.Name);
				return (application == null) ? error.InvalidParameters(Response) : Response.AsJson(new { ApplicationId = application.Id });

			};

			Post["/transfer"] = _ =>
			{
				if (Params.AreMissing("SingleUseToken", "Name", "Id")) return error.MissingParameters(Response);
				if (!Params.Id.IsGuid() || !Params.SingleUseToken.IsGuid()) return error.InvalidParameters(Response);
				if (!applicationService.AuthoriseSingleUseToken(Params.SingleUseToken)) return error.PermissionDenied(Response);

				var application = applicationService.Transfer(Params.Name, Params.Id);
				return (application == null) ? error.InvalidParameters(Response) : Response.AsJson(new { ApplicationId = application.Id });

			};
		}
	}
}
