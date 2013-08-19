using Nancy;
using Nancy.Security;
using ThumbsUp.Client;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class ApplicationModule : _BaseModule
	{
		public ApplicationModule()
			: base("/application")
		{
			this.RequiresAuthentication();

			//Post["/register"] = _ => Response.AsJson(ThumbsUpApi.RegisterApplication(ThumbsUpApplicationId, Params.Name).Data);
			//Post["/transfer"] = _ => Response.AsJson(ThumbsUpApi.TransferApplication(ThumbsUpApplicationId, Params.Name, Params.Id).Data);
		}
	}
}
