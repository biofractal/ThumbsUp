using System;
using ThumbsUp.Service;
using Topshelf;

namespace ThumbsUp.Service
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			HostFactory.Run(config => {
				config.Service<SelfHost>(service =>
				{
					service.ConstructUsing(host => new SelfHost());
					service.WhenStarted(host => host.Start());
					service.WhenStopped(host => host.Stop());
				});
				config.RunAsLocalSystem();
				config.StartAutomatically();
				config.SetDescription("ThumbsUp - Central security hub for all GoodPractice services and applications");
				config.SetDisplayName("ThumbsUp");
				config.SetServiceName("ThumbsUp");
			});
		}
	}
}

