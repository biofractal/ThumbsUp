using Nancy.ViewEngines.Razor;
using ThumbsUp.Nancy.FormsAuthentication;

namespace ThumbsUp.Demo.Nancy
{

	public static class Extensions
	{
		public static ThumbsUpNancyUser GetUser<T>(this HtmlHelpers<T> html)
		{
			return (ThumbsUpNancyUser)html.RenderContext.Context.CurrentUser;
		}
	}

}