using Nancy.Security;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class RootModule : _BaseModule
	{
		public RootModule()
		{
			this.RequiresAuthentication();
			Get["/"] = _ => View["ProtectedView"];
		}
	}
}
