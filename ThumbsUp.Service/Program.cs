using System;
using System.Runtime.InteropServices;
using System.Threading;
using ThumbsUp.Service;
using Topshelf;

namespace ThumbsUp.Service
{
	public class Program
	{
		//Prevent more than one instance running
		private static Mutex mutex = new Mutex(true, @"Global\{9b41dbe3-4e1a-4b6b-aa5a-ded9c7ef29f9}");

		[STAThread]
		public static void Main()
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				try
				{
					HostFactory.Run(config =>
					{
						config.Service<SelfHost>(service =>
						{
							service.ConstructUsing(host => new SelfHost());
							service.WhenStarted(host => host.Start());
							service.WhenStopped(host => host.Stop());
							service.WhenShutdown(host => host.Stop());
						});
						config.RunAsLocalSystem();
						config.StartAutomatically();
						config.SetDescription("ThumbsUp - A central security hub for all GoodPractice http services and applications");
						config.SetDisplayName("ThumbsUp Security Service");
						config.SetServiceName("ThumbsUp");
					});
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			else
			{
				Console.WriteLine("ThumbsUp Service already running.");
				Environment.Exit(0);
			}
		}
	}
}

