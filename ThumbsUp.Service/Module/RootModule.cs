using Nancy;
using Nancy.Helper;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class RootModule : NancyModule
	{
		public RootModule(UserService userService, ApplicationService applicationService)
		{
			Get["/"] = _ =>
			{
				return "ThumbsUp Security Service is Running";
			};
		}
	}
}
