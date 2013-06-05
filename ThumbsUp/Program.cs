using System;
using ThumbsUp.Service;
using Topshelf;

namespace ThumbsUp
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			HostFactory.Run(config => {
				config.Service<ThumbsUpService>(service => {
					service.ConstructUsing(name => new ThumbsUpService());
					service.WhenStarted(thumbsUp => thumbsUp.Start());
					service.WhenStopped(thumbsUp => thumbsUp.Stop());
					service.AfterStartingService(() => ThumbsUpService.RunConsole());
				});
				config.RunAsLocalSystem();
				config.StartAutomatically();
			});
		}
	}
}

