using Nancy;
using Nancy.Helper;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class RootModule : NancyModule
	{
		public RootModule()
		{
			Get["/"] = _ =>
			{
				return "ThumbsUp Security Service is Running";
			};

			Get["/error/{code}"] = url =>
			{
				var code = (int)url.code;
				return Response.AsJson(new { ErrorCode = code, ErrorMessage = ErrorService.Decode(code) });
			};

		}
	}
}
