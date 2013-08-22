
using RestSharp;
using System.Net;

namespace ThumbsUp.Client
{
	public class ThumbsUpResult
	{
		public bool Success { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public ThumbsUpResponse Data { get; set; }
		public ThumbsUpResult(IRestResponse<ThumbsUpResponse> response)
		{
			this.Success = response.StatusCode == HttpStatusCode.OK;
			this.StatusCode = response.StatusCode;
			this.Data = (response.Data != null) ? response.Data : new ThumbsUpResponse();
		}
		public string GetMessage()
		{
			return (Data.ErrorCode == 0) ? "ThumbsUp is not responding" : Data.ErrorMessage;
		}
	}
}
