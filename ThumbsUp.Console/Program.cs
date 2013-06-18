using System;
using ThumbsUp.Console.Services;
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
				C.WriteLine();

				switch (C.ReadLine())
				{		
					case "1":
						C.WriteLine("Quiting the Console");
						Environment.Exit(0);
						break;	
					case "2":
						ThumbsUpService.CheckServiceIsRunning();
						break;			
					case "3":
						ThumbsUpService.RegisterApplication();
						break;
					case "4":
						ThumbsUpService.RegisterUser();
						break;
					case "5":
						ThumbsUpService.UserLogin();
						break;
					case "6":
						ThumbsUpService.UserFromKey();
						break;
					case "7":
						ThumbsUpService.UserLogout();
						break;

				}
			}
		}
	}
}
