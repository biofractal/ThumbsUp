using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using ThumbsUp.Client;

namespace ThumbsUp.Nancy.FormsAuthentication
{
	public interface IThumbsUpNancyApi : IThumbsUpApi, IUserMapper { }

	public class ThumbsUpNancyApi : ThumbsUpApi, IThumbsUpNancyApi, IUserMapper
	{
		public IUserIdentity GetUserFromIdentifier(Guid thumbKey, NancyContext context)
		{
			var response = GetUser(thumbKey);
			if (!response.Success) return null;
			var user = new ThumbsUpNancyUser(response.Data.User) { ThumbKey = thumbKey };
			return user;
		}
	}
}
