using Nancy;
using Nancy.Security;
using ThumbsUp.Nancy.FormsAuthentication;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class UserModule : _BaseModule
	{
		public UserModule(IThumbsUpNancyApi thumbsUp)
			: base("/user")
		{
			this.RequiresAuthentication();

			Post["/create"] = _ => Response.AsJson(thumbsUp.CreateUser(Params.UserName, Params.Email).Data);
		}
	}
}
