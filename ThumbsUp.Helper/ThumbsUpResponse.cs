using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbsUp.Helper
{
	public class ThumbsUpResponse
	{
		public ThumbsUpUser User { get; set; }
		public string Password { get; set; }
		public string ApplicationId { get; set; }
		public Guid? ThumbKey { get; set; }
		public int ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
		public string ThumbsUpsUrl { get; set; }
		public string Token { get; set; }
	}
}
