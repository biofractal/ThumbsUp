using RestSharp;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ThumbsUp.Helper
{
	public class ThumbsUpResult
	{
		public bool Success { get; set; }
		public Nancy.HttpStatusCode StatusCode { get; set; }
		public ThumbsUpResponse Data { get; set; }
		public ThumbsUpResult(IRestResponse<ThumbsUpResponse> response)
		{
			this.Success = response.StatusCode==System.Net.HttpStatusCode.OK;
			this.StatusCode = (Nancy.HttpStatusCode)response.StatusCode;
			this.Data = (response.Data != null) ? response.Data : new ThumbsUpResponse();
		}
		public string GetMessage()
		{
			return (Data.ErrorCode == 0) ? "ThumbsUp is not responding" : Data.ErrorMessage;
		}
	}
}
