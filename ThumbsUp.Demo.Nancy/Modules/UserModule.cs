using Nancy;
using Nancy.Security;
using ThumbsUp.Helper;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule()
			: base("/user")
		{
			this.RequiresAuthentication();

			Post["/create"] = _ => Response.AsJson(ThumbsUpApi.CreateUser(ThumbsUpApplicationId, Params.UserName, Params.Email).Data);
		}
	}
}
