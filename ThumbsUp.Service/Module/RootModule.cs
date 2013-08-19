using Nancy;
using Nancy.Helper;
using ThumbsUp.Domain;

namespace ThumbsUp.Service.Module
{
	public class RootModule : _BaseModule
	{
		public RootModule(IErrorService error)
		{
			Get["/"] = _ =>
			{
				return "ThumbsUp Security Service is Running";
			};

			Get["/error/{code}"] = url =>
			{
				if (Params.AreMissing("Code")) return error.MissingParameters(Response);
				return Response.AsJson(new { ErrorCode = Params.Code, ErrorMessage = error.Decode(Params.Code)});
			};

		}
	}
}
