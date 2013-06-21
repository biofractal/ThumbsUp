using Nancy.Helper;
using System;
using System.Windows.Forms;
using C = System.Console;

namespace ThumbsUp.Console
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			while (true)
			{
				C.WriteLine();
				C.WriteLine("Select an Action");
				C.WriteLine();
				C.WriteLine("   1 - Quit");
				C.WriteLine("   2 - Check the service is running");
				C.WriteLine("   3 - Register Application");
				C.WriteLine("   4 - Register User");
				C.WriteLine("   5 - User Login");
				C.WriteLine("   6 - User From Key");
				C.WriteLine("   7 - User Logout");
				C.WriteLine("   8 - Validate Key");
				C.WriteLine("   9 - Validate UserName");
				C.WriteLine();

				switch (C.ReadLine())
				{
					case "1":
						C.WriteLine("Quiting the Console");
						Environment.Exit(0);
						break;
					case "2":
						CheckServiceIsRunning();
						break;
					case "3":
						RegisterApplication();
						break;
					case "4":
						RegisterUser();
						break;
					case "5":
						UserLogin();
						break;
					case "6":
						UserFromKey();
						break;
					case "7":
						UserLogout();
						break;
					case "8":
						ValidateKey();
						break;
					case "9":
						ValidateUserName();
						break;
				}
			}
		}

		public static void CheckServiceIsRunning()
		{
			C.WriteLine("Checking the ThumbsUp Service");
			var result = ThumbsUpApi.CheckServiceIsRunning();
			var uri = result.Data.ThumbsUpsUrl;
			C.WriteLine(result.Success ? "Success: The service is running on " + uri : "Failure: Cannot locate a running service at " + uri);
		}

		public static void UserLogin()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Password?");
			var password = C.ReadLine();
			var result = ThumbsUpApi.ValidateUser(username, password);
			if (!IsError(result))
			{
				C.WriteLine("Success. The user has been logged in");
				CopyToClipboard((string)result.Data.ThumbKey.ToString(), "thumbkey");
			}
		}

		public static void UserLogout()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.Logout(thumbKey);
			if (!IsError(result)) C.WriteLine("Success. The user has been logged out");
		}

		public static void RegisterUser()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Email?");
			var email = C.ReadLine();
			var result = ThumbsUpApi.CreateUser(username, email);
			if (!IsError(result))
			{
				C.WriteLine("Success. The new User has been registered");
				CopyToClipboard((string)result.Data.Password, "Password");
			}
		}

		public static void RegisterApplication()
		{
			C.WriteLine("Application Name?");
			var name = C.ReadLine();
			var result = ThumbsUpApi.CreateApplication(name);
			if (!IsError(result))
			{
				C.WriteLine("Success. The new Application has been registered");
				CopyToClipboard((string)result.Data.ApplicationId, "Application Id");
			}
		}

		public static void UserFromKey()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.GetUserFromIdentifier(thumbKey);
			if (!IsError(result)) C.WriteLine(string.Format("Success. UserName = {0} : Email = {1}", result.Data.User.UserName, result.Data.User.Email));
		}

		public static void ValidateKey()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.ValidateKey(thumbKey);
			if (!IsError(result)) C.WriteLine("Success. The thumbKey exists");
		}

		public static void ValidateUserName()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			var result = ThumbsUpApi.ValidateUserName(username);
			if (!IsError(result)) C.WriteLine("The username is valid. It has not yet been used");
		}

		private static Guid GetThumbKey()
		{
			C.WriteLine("thumbKey?");
			Guid thumbKey;
			while (!Guid.TryParse(C.ReadLine(), out thumbKey))
			{
				C.WriteLine("thumbKey?");
			};
			return thumbKey;
		}

		private static void CopyToClipboard(string item, string name = "")
		{
			if (string.IsNullOrEmpty(item)) return;
			Clipboard.SetText(item);
			if (string.IsNullOrWhiteSpace(name)) return;
			C.WriteLine(string.Format("This {0} has been copied to the clipboard : {1}", name, item));
		}

		private static bool IsError(ThumbsUpApi.ThumbsUpResult result)
		{
			var isError = !result.Success;
			if (isError) C.WriteLine("Failure: " + result.Data.ErrorMessage);
			return isError;
		}
	}
}
