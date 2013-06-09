
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using System.Windows.Forms;
using C = System.Console;

namespace ThumbsUp.Console.Services
{
	public static class ThumbsUpService
	{
		private class ThumbsUpResponse
		{
			public string Password { get; set; }
			public string ApplicationId { get; set; }
			public string ThumbKey { get; set; }
		}

		private static readonly string ThumbsUpsApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Url"];

		public static void CheckServiceIsRunning()
		{
			C.WriteLine("Checking the ThumbsUp Service on " + ThumbsUpsUrl);
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("/", Method.GET);
			var response = client.Execute<ThumbsUpResponse>(request);
			C.WriteLine((response.StatusCode == HttpStatusCode.OK)? "Success: The service is running fine" : "Failure: Cannot locate a running service at this address");
		}

		public static void CheckLogin()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Password?");
			var password = C.ReadLine();
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("login", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			var response = client.Execute<ThumbsUpResponse>(request);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var thumbKey = response.Data.ThumbKey;
				C.WriteLine("The User is Valid. ThumbKey : " + thumbKey);
			}
			else
			{
				C.WriteLine("Invalid UserName or Password");
			}
		}

		public static void CreateUser()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Email?");
			var email = C.ReadLine();
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("user/create", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			Clipboard.SetText(client.Execute<ThumbsUpResponse>(request).Data.Password);
			C.WriteLine("The Password has been copied to the clipboard");
		}

		public static void CreateApplication()
		{
			C.WriteLine("Application Name?");
			var name = C.ReadLine();
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("application/create", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("name", name);
			Clipboard.SetText(client.Execute<ThumbsUpResponse>(request).Data.ApplicationId);
			C.WriteLine("The Application Id has been copied to the clipboard");
		}

	}
}
