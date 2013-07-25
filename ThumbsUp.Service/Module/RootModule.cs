using Nancy;
using Nancy.Helper;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;

namespace ThumbsUp.Service.Module
{
	public class RootModule : _BaseModule
	{
		public RootModule(IErrorService errorService)
		{
			Get["/"] = _ =>
			{
				return "ThumbsUp Security Service is Running";
			};

			Get["/error/{code}"] = url =>
			{
				if (Params.AreMissing("Code")) return Params.Missing(Response);
				return Response.AsJson(new { ErrorCode = Params.Code, ErrorMessage = errorService.Decode(Params.Code)});
			};

		}
	}
}
