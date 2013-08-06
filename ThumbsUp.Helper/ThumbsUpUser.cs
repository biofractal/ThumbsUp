using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbsUp.Helper
{
	public class ThumbsUpUser : IUserIdentity
	{
		#region ThumbsUp
		public Guid ThumbKey { get; set; }
		public string Email { get; set; }
		#endregion

		#region IUserIdentity
		public string Id { get; set; }
		public string UserName { get; set; }
		public IEnumerable<string> Claims { get; set; }
		#endregion
	}
}
