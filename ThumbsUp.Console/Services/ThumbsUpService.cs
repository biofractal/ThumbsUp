
using Nancy.Helper;
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
		public static void CheckServiceIsRunning()
		{
			C.WriteLine("Checking the ThumbsUp Service");
			var result = ThumbsUpApi.CheckServiceIsRunning();
			var success = result.Item1;
			var uri = result.Item2;
			C.WriteLine( success? "Success: The service is running on " + uri : "Failure: Cannot locate a running service at " + uri);
		}


		public static void UserLogin()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Password?");
			var password = C.ReadLine();
			var thumbkey = ThumbsUpApi.ValidateUser(username, password);
			C.WriteLine((thumbkey != null) ? "The User is Valid" : "Invalid UserName or Password");
			CopyToClipboard(thumbkey.ToString(), "Thumbkey");
		}

		public static void UserLogout()
		{			
			C.WriteLine("ThumbKey?");
			Guid thumbkey;
			while(!Guid.TryParse(C.ReadLine(), out thumbkey))
			{
				C.WriteLine("ThumbKey?");
			};
			var success = ThumbsUpApi.Logout(thumbkey);
			C.WriteLine("Success = " + success);
		}

		public static void RegisterUser()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Email?");
			var email = C.ReadLine();
			var password = ThumbsUpApi.CreateUser(username, email);
			CopyToClipboard(password, "Password");
		}

		public static void RegisterApplication()
		{
			C.WriteLine("Application Name?");
			var name = C.ReadLine();
			var applicationId = ThumbsUpApi.CreateApplication(name);
			CopyToClipboard(applicationId, "Application Id");
		}
		public static void UserFromKey()
		{			
			C.WriteLine("ThumbKey?");
			Guid thumbkey;
			while(!Guid.TryParse(C.ReadLine(), out thumbkey))
			{
				C.WriteLine("ThumbKey?");
			};
			var user = ThumbsUpApi.GetUserFromIdentifier(thumbkey);
			C.WriteLine((user == null)? "Invalid ThumbKey" : "Success. User Name is " + user.UserName);
		}

		private static void CopyToClipboard(string item, string name="")
		{
			if (string.IsNullOrEmpty(item)) return;
			Clipboard.SetText(item);
			if(string.IsNullOrWhiteSpace(name)) return;
			C.WriteLine(string.Format("This {0} has been copied to the clipboard : {1}", name, item));
		}


	}
}
