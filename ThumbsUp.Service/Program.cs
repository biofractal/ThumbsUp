using System;
using ThumbsUp.Service;
using Topshelf;

namespace ThumbsUp
{
	public class Program
	{
		public static void Main()
		{
			HostFactory.Run(config => {
				config.Service<SelfHost>(service =>
				{
					service.ConstructUsing(name => new SelfHost());
					service.WhenStarted(thumbsUp => thumbsUp.Start());
					service.WhenStopped(thumbsUp => thumbsUp.Stop());
				});
				config.RunAsLocalSystem();
				config.StartAutomatically();
			});
		}
	}
}

