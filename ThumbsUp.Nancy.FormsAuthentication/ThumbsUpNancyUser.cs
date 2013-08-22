using Nancy.Security;
using System;
using System.Collections.Generic;
using ThumbsUp.Client;

namespace ThumbsUp.Nancy.FormsAuthentication
{
	public class ThumbsUpNancyUser : IThumbsUpUser, IUserIdentity
	{
		private readonly IThumbsUpUser User;

		public ThumbsUpNancyUser(IThumbsUpUser user) 
		{
			User = user;
		}

		public string Id
		{
			get{return User.Id;}
			set{User.Id = value;}
		}

		public Guid ThumbKey
		{
			get { return User.ThumbKey; }
			set { User.ThumbKey = value; }
		}

		public string UserName
		{
			get { return User.UserName; }
			set { User.UserName = value; }
		}

		public string Email
		{
			get { return User.Email; }
			set { User.Email = value; }
		}

		public IEnumerable<string> Claims { get; set; }
	}
}
