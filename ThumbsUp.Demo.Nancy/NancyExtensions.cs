using Nancy.ViewEngines.Razor;
using ThumbsUp.Client;

namespace ThumbsUp.Demo.Nancy
{
	public static class NancyExtensions
	{
		public static ThumbsUpUser GetUser<T>(this HtmlHelpers<T> html)
		{
			return (ThumbsUpUser)html.RenderContext.Context.CurrentUser;
		}
	}
}
