using System;
using Topshelf;

namespace ThumbsUp.Service
{
	public class Program
	{


		[STAThread]
		public static void Main()
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
				config.SetDescription("ThumbsUp - A central security hub for http services and applications");
				config.SetDisplayName("ThumbsUp Security Service");
				config.SetServiceName("ThumbsUp");
			});

		}
	}
}

