using Nancy;
using ThumbsUp.Service;

namespace ThumbsUp.Module
{
	public class RootModule : NancyModule
	{
		public RootModule(UserService userService, ApplicationService applicationService)
		{
			Get["/"] = _ =>  "ThumbsUp Security Service is Running";
		}
	}
}
