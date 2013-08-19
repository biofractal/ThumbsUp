

namespace ThumbsUp.Helper
{
	using Nancy;
	using Nancy.Authentication.Forms;
	using Nancy.Security;
	using RestSharp;
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	public class ThumbsUpApi : IUserMapper
	{
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Service.Uri"];
		private static readonly RestClient Client = new RestClient(ThumbsUpsUrl);

		public IUserIdentity GetUserFromIdentifier(Guid thumbKey, NancyContext context)
		{
			var applicationId = GetParam(context.Request, "applicationid");
			var response = ThumbsUpApi.GetUserFromIdentifier(applicationId, thumbKey);
			if (!response.Success) return null;
			var user = response.Data.User;
			user.ThumbKey = thumbKey;
			return user;
		}

		public string GetParam(Request request, string name)
		{
			if (request.Query[name].HasValue) return (string)request.Query[name];
			if (request.Form[name].HasValue) return (string)request.Form[name];
			return string.Empty;
		}

		public static ThumbsUpResult CheckServiceIsRunning(string applicationId)
		{
			var request = MakeRequest(applicationId, "/", Method.GET);
			var result = new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
			result.Data.ThumbsUpsUrl = ThumbsUpsUrl;
			return result;
		}

		public static ThumbsUpResult GetUserFromIdentifier(string applicationId, Guid thumbKey)
		{
			var request = MakeRequest(applicationId, "/user/get");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateUser(string applicationId, string username, string password)
		{
			var request = MakeRequest(applicationId, "user/validate");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateUserName(string applicationId, string username)
		{
			var request = MakeRequest(applicationId, "user/validate/name");
			request.AddParameter("username", username);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateKey(string applicationId, Guid thumbKey)
		{
			var request = MakeRequest(applicationId, "user/validate/thumbkey");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult Logout(string applicationId, Guid thumbKey)
		{
			var request = MakeRequest(applicationId, "user/logout");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult CreateUser(string applicationId, string username, string email)
		{
			var request = MakeRequest(applicationId, "user/create");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult RegisterApplication(string applicationId, string singleUseToken, string name)
		{
			var request = MakeRequest(applicationId, "application/register");
			request.AddParameter("singleUseToken", singleUseToken);
			request.AddParameter("name", name);
			
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult TransferApplication(string applicationId, string singleUseToken, string name, string id)
		{
			var request = MakeRequest(applicationId, "application/transfer");
			request.AddParameter("singleUseToken", singleUseToken);
			request.AddParameter("name", name);
			request.AddParameter("id", id);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult GetErrorMessage(string applicationId, int errorCode)
		{
			var request = MakeRequest(applicationId, "error/" + errorCode, Method.GET);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}


		public static ThumbsUpResult ResetPassword(string applicationId, string username, string password)
		{
			var request = MakeRequest(applicationId, "user/reset/password");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ForgotPasswordRequest(string applicationId, string username, string email)
		{
			var request = MakeRequest(applicationId, "user/forgot-password/request");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ForgotPasswordReset(string applicationId, string username, string token)
		{
			var request = MakeRequest(applicationId, "user/forgot-password/reset");
			request.AddParameter("username", username);
			request.AddParameter("token", token);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		private static RestRequest MakeRequest(string applicationId, string url, Method method = Method.POST)
		{
			var request = new RestRequest(url, method);
			request.AddParameter("applicationid", applicationId);
			return request;
		}

	}
}
