using Nancy.Security;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class HomeModule : _BaseModule
	{
		public HomeModule()
		{
			this.RequiresAuthentication();
			Get["/"] = _ => View["HomeView"];
			Get["/home"] = _ => View["HomeView"];
		}
	}
}
