using RestSharp;
using System;
using System.Configuration;


namespace ThumbsUp.Client
{
	public interface IThumbsUpApi
	{
		ThumbsUpResult CheckServiceIsRunning(string applicationId = null);
		ThumbsUpResult GetUser(Guid thumbKey, string applicationId = null);
		ThumbsUpResult ValidateUser(string username, string password, string applicationId = null);
		ThumbsUpResult ValidateUserName(string username, string applicationId = null);
		ThumbsUpResult ValidateKey(Guid thumbKey, string applicationId = null);
		ThumbsUpResult Logout(Guid thumbKey, string applicationId = null);
		ThumbsUpResult CreateUser(string username, string email, string applicationId = null);
		ThumbsUpResult RegisterApplication(string singleUseToken, string name, string applicationId = null);
		ThumbsUpResult TransferApplication(string singleUseToken, string name, string id, string applicationId = null);
		ThumbsUpResult GetErrorMessage(int errorCode, string applicationId = null);
		ThumbsUpResult ResetPassword(string username, string password, string applicationId = null);
		ThumbsUpResult ForgotPasswordRequest(string username, string email, string applicationId = null);
		ThumbsUpResult ForgotPasswordReset(string username, string token, string applicationId = null);
	}

	public class ThumbsUpApi : IThumbsUpApi
	{
		private readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Service.Uri"];
		private readonly string ThumbsUpsApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];
		private readonly RestClient Client;

		public ThumbsUpApi()
		{
			Client = new RestClient(ThumbsUpsUrl);
		}

		public ThumbsUpResult CheckServiceIsRunning(string applicationId = null)
		{
			var request = MakeRequest(applicationId, "/", Method.GET);
			var result = new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
			result.Data.ThumbsUpsUrl = ThumbsUpsUrl;
			return result;
		}

		public ThumbsUpResult GetUser(Guid thumbKey, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "/user/get");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ValidateUser(string username, string password, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/validate");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ValidateUserName(string username, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/validate/name");
			request.AddParameter("username", username);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ValidateKey(Guid thumbKey, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/validate/thumbkey");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult Logout(Guid thumbKey, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/logout");
			request.AddParameter("thumbkey", thumbKey);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult CreateUser(string username, string email, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/create");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult RegisterApplication(string singleUseToken, string name, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "application/register");
			request.AddParameter("singleUseToken", singleUseToken);
			request.AddParameter("name", name);

			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult TransferApplication(string singleUseToken, string name, string id, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "application/transfer");
			request.AddParameter("singleUseToken", singleUseToken);
			request.AddParameter("name", name);
			request.AddParameter("id", id);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult GetErrorMessage(int errorCode, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "error/" + errorCode, Method.GET);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ResetPassword(string username, string password, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/reset/password");
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ForgotPasswordRequest(string username, string email, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/forgot-password/request");
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		public ThumbsUpResult ForgotPasswordReset(string username, string token, string applicationId = null)
		{
			var request = MakeRequest(applicationId, "user/forgot-password/reset");
			request.AddParameter("username", username);
			request.AddParameter("token", token);
			return new ThumbsUpResult(Client.Execute<ThumbsUpResponse>(request));
		}

		private RestRequest MakeRequest(string applicationId, string url, Method method = Method.POST)
		{
			applicationId = applicationId ?? ThumbsUpsApplicationId;
			var request = new RestRequest(url, method);
			request.AddParameter("applicationid", applicationId);
			return request;
		}

	}
}
