using Nancy;
using Nancy.Security;
using ThumbsUp.Client;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule()
			: base("/user")
		{
			this.RequiresAuthentication();

			Post["/create"] = _ => Response.AsJson(ThumbsUpApi.CreateUser(Params.UserName, Params.Email).Data);
		}
	}
}
