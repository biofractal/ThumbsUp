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
				C.WriteLine("   3 - Check a User's credentials");
				C.WriteLine("   4 - Create a new User");
				C.WriteLine("   5 - Register a new Application");
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
						ThumbsUpService.CheckUserCredentials();
						break;
					case "4":
						ThumbsUpService.CreateUser();
						break;
					case "5":
						ThumbsUpService.CreateApplication();
						break;

				}
			}
		}
	}
}
