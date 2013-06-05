
using Nancy.Hosting.Self;
using RestSharp;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace ThumbsUp.Service
{
	class ThumbsUpResponse
	{
		public string Password { get; set; }
		public string ApplicationId { get; set; }
	}

	public class ThumbsUpService
	{
		private static readonly string ThumbsUpsApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Url"];
		private NancyHost _nancyHost;

		public void Start()
		{
			_nancyHost = new NancyHost(new Uri(ThumbsUpsUrl));
			_nancyHost.Start();
			Console.WriteLine("ThumbsUp API Running on {0}. Hit Control-C to stop.", ThumbsUpsUrl);
		}

		public void Stop()
		{
			_nancyHost.Stop();
			Console.WriteLine("ThumbsUp Service has successfully Stopped.");
		}

		public static void RunConsole()
		{
				Console.WriteLine();
				Console.WriteLine("Select an Action");
				Console.WriteLine();
				Console.WriteLine("   1 - Create User");
				Console.WriteLine("   2 - Create Application");
				Console.WriteLine();

				switch (Console.ReadLine())
				{
					case "1":
						CreateUser();
						break;
					case "2":
						CreateApplication();
						break;
				}
		}

		public static void CreateUser()
		{
			Console.WriteLine("UserName?");
			var username = Console.ReadLine();
			Console.WriteLine("Email?");
			var email = Console.ReadLine();
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("user/create", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("username", username);
			request.AddParameter("email", email);
			Clipboard.SetText(client.Execute<ThumbsUpResponse>(request).Data.Password);
			Console.WriteLine("The Password has been copied to the clipboard");
		}

		public static void CreateApplication()
		{
			Console.WriteLine("Application Name?");
			var name = Console.ReadLine();
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("application/create", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("name", name);
			Clipboard.SetText(client.Execute<ThumbsUpResponse>(request).Data.ApplicationId);
			Console.WriteLine("The Application Id has been copied to the clipboard");
		}
	}
}
