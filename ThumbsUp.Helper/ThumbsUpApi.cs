

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
		private static readonly string ThumbsUpsApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Service.Uri"];
		private static readonly RestClient Client = new RestClient(ThumbsUpsUrl);

		public IUserIdentity GetUserFromIdentifier(Guid thumbKey, NancyContext context)
		{
			var response = ThumbsUpApi.GetUserFromIdentifier(thumbKey);
			if (!response.Success) return null;
			var user = response.Data.User;
			user.ThumbKey = thumbKey;
			return user;
		}

		public static ThumbsUpResult CheckServiceIsRunning()
		{
			var request = MakeRequest("/", Method.GET);
			var result = new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
			result.Data.ThumbsUpsUrl = ThumbsUpsUrl;
			return result;
		}

		public static ThumbsUpResult GetUserFromIdentifier(Guid thumbKey)
		{
			var request = MakeRequest("/user/get");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateUser(string username, string password)
		{
			var request = MakeRequest("user/validate");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateUserName(string username)
		{
			var request = MakeRequest("user/validate/name");
			request.AddParameter("username", username);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ValidateKey(Guid thumbKey)
		{
			var request = MakeRequest("user/validate/thumbkey");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult Logout(Guid thumbKey)
		{
			var request = MakeRequest("user/logout");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult CreateUser(string username, string email)
		{
			var request = MakeRequest("user/create");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult RegisterNewApplication(string name)
		{
			var request = MakeRequest("application/register/new");
			request.AddParameter("name", name);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult RegisterExistingApplication(string name, string applicationId)
		{
			var request = MakeRequest("application/register/existing");
			request.AddParameter("name", name);
			request.AddParameter("id", applicationId);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult GetErrorMessage(int errorCode)
		{
			var request = MakeRequest("error/" + errorCode, Method.GET);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}


		public static ThumbsUpResult ResetPassword(string username, string password)
		{
			var request = MakeRequest("user/reset/password");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ForgotPasswordRequest(string username, string email)
		{
			var request = MakeRequest("user/forgot-password/request");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public static ThumbsUpResult ForgotPasswordReset(string username, string token)
		{
			var request = MakeRequest("user/forgot-password/reset");
			request.AddParameter("username", username);
			request.AddParameter("token", token);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		private static RestRequest MakeRequest(string url, Method method = Method.POST)
		{
			var request = new RestRequest(url, method);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			return request;
		}

	}
}
