
using Nancy.Helper;
using Raven.Client;
using System;
using System.Linq;
using System.Windows.Forms;
using ThumbsUp.Client;
using C = System.Console;

namespace ThumbsUp.Console
{
	public class Program
	{
		public static string ConsoleApplicationId = ThumbsUpAdmin.GetConsoleApplicationId(Environment.MachineName);

		[STAThread]
		public static void Main(string[] args)
		{
			while (true)
			{
				C.WriteLine();
				C.WriteLine("ThumbsUp Security Service");
				C.WriteLine("=========================");
				C.WriteLine();
				C.WriteLine("   0 - Quit");
				C.WriteLine("   1 - Check the service is running");
				C.WriteLine("   2 - Register New Application");
				C.WriteLine("   3 - Register Existing Application");
				C.WriteLine("   4 - Register User");
				C.WriteLine("   5 - User Login");
				C.WriteLine("   6 - User From Key");
				C.WriteLine("   7 - User Logout");
				C.WriteLine("   8 - Validate Key");
				C.WriteLine("   9 - Validate UserName");
				C.WriteLine("   a - Decode Error");
				C.WriteLine("   b - User Reset Password");
				C.WriteLine("   c - Forgot Password Request");
				C.WriteLine("   d - Forgot Password Reset");
				C.WriteLine();
				C.WriteLine("   y - Install & Start the Service");
				C.WriteLine("   z - Uninstall the Service");
				C.WriteLine();
				var option = C.ReadLine();

				C.WriteLine();
				C.WriteLine("   ------------------------------------");
				C.WriteLine();
				switch (option)
				{
					case "0":
						Quit();
						break;
					case "1":
						CheckServiceIsRunning();
						break;
					case "2":
						RegisterNewApplication();
						break;
					case "3":
						RegisterExistingApplication();
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
					case "a":
						GetErrorMessage();
						break;
					case "b":
						UserResetPassword();
						break;
					case "c":
						ForgotPasswordRequest();
						break;
					case "d":
						ForgotPasswordReset();
						break;
					case "y":
						ServiceCommand("install start");
						break;
					case "z":
						ServiceCommand("uninstall");
						break;
				}

				C.WriteLine();
				C.WriteLine("   ------------------------------------");
				C.WriteLine();
				C.WriteLine("   0   - Quit");
				C.WriteLine();
				C.WriteLine("  [ Press any other key for Main Menu ]");
				C.WriteLine();
				if (C.ReadLine() == "0") Quit();
			}
		}

		public static void Quit()
		{
			C.WriteLine("Quiting the Console");
			Environment.Exit(0);
		}

		public static void CheckServiceIsRunning()
		{
			C.WriteLine("Checking the ThumbsUp Service");
			var result = ThumbsUpApi.CheckServiceIsRunning(ConsoleApplicationId);
			var uri = result.Data.ThumbsUpsUrl;
			C.WriteLine(result.Success ? "Success: The service is running on " + uri : "Failure: Cannot locate a running service at " + uri);
		}

		public static void UserLogin()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Password?");
			var password = C.ReadLine();
			var result = ThumbsUpApi.ValidateUser(username, password, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The user has been logged in");
				CopyToClipboard((string)result.Data.ThumbKey.ToString(), "thumbkey");
			}
		}

		public static void UserLogout()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.Logout(thumbKey, ConsoleApplicationId);
			if (!IsError(result)) C.WriteLine("Success. The user has been logged out");
		}

		public static void RegisterUser()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Email?");
			var email = C.ReadLine();
			var result = ThumbsUpApi.CreateUser(username, email, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The new User has been registered");
				CopyToClipboard((string)result.Data.Password, "Password");
			}
		}

		public static void RegisterNewApplication()
		{
			C.WriteLine("Application Name?");
			var name = C.ReadLine();
			var singleUseToken = ThumbsUpAdmin.GenerateSingleUseToken();
			var result = ThumbsUpApi.RegisterApplication(singleUseToken, name, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The new Application has been registered");
				CopyToClipboard((string)result.Data.ApplicationId, "Application Id");
			}
		}
		public static void RegisterExistingApplication()
		{
			C.WriteLine("Application Name?");
			var name = C.ReadLine();
			C.WriteLine("Application Id?");
			var applicationId = C.ReadLine();
			var singleUseToken = ThumbsUpAdmin.GenerateSingleUseToken();
			var result = ThumbsUpApi.TransferApplication(singleUseToken, name, applicationId, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The existing Application has been registered");
				CopyToClipboard((string)result.Data.ApplicationId, "Application Id");
			}
		}
		public static void UserFromKey()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.GetUserFromIdentifier(thumbKey, ConsoleApplicationId);
			if (!IsError(result)) C.WriteLine(string.Format("Success. UserName = {0} : Email = {1}", result.Data.User.UserName, result.Data.User.Email));
		}

		public static void ValidateKey()
		{
			var thumbKey = GetThumbKey();
			var result = ThumbsUpApi.ValidateKey(thumbKey, ConsoleApplicationId);
			if (!IsError(result)) C.WriteLine("Success. The thumbKey exists");
		}

		public static void ValidateUserName()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			var result = ThumbsUpApi.ValidateUserName(username, ConsoleApplicationId);
			if (!IsError(result)) C.WriteLine("The username is valid. It has not yet been used");
		}

		public static void GetErrorMessage()
		{
			C.WriteLine("Error Code?");
			var errorCode = C.ReadLine();
			var result = ThumbsUpApi.GetErrorMessage(int.Parse(errorCode), ConsoleApplicationId);
			if (!IsError(result)) C.WriteLine("Success. Message = " + result.Data.ErrorMessage);
		}

		public static void UserResetPassword()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Password?");
			var password = C.ReadLine();
			var result = ThumbsUpApi.ResetPassword(username, password, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The password has been reset");
				CopyToClipboard((string)result.Data.Password, "password");
			}
		}

		public static void ForgotPasswordRequest()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Email?");
			var email = C.ReadLine();
			var result = ThumbsUpApi.ForgotPasswordRequest(username, email, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The request has been processed");
				CopyToClipboard((string)result.Data.Token, "Request Token");
			}
		}

		public static void ForgotPasswordReset()
		{
			C.WriteLine("UserName?");
			var username = C.ReadLine();
			C.WriteLine("Request Token?");
			var token = C.ReadLine();
			var result = ThumbsUpApi.ForgotPasswordReset(username, token, ConsoleApplicationId);
			if (!IsError(result))
			{
				C.WriteLine("Success. The password has been reset");
				CopyToClipboard((string)result.Data.Password, "Password");
			}
		}

		public static void ServiceCommand(string arguments)
		{
			var stdOut = ExternalProcess.Run("ThumbsUp.Service.exe", arguments);
			C.WriteLine(stdOut);
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

		private static bool IsError(ThumbsUpResult result)
		{
			var isError = !result.Success;
			if (isError) C.WriteLine("Failure: " + result.Data.ErrorMessage);
			return isError;
		}
	}
}
