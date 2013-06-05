using Nancy;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class ApplicationModule : NancyModule
	{
		public ApplicationModule(UserService userService, ApplicationService applicationService)
			: base("/application")
		{
			Post["/create"] = _ =>
			{
				var name = (string)Request.Form.name;
				var application = applicationService.Create(name);
				return Response.AsJson(new { ApplicationId = application.Id });
			};
		}
	}
}
