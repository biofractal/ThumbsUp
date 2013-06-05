
using Nancy.Hosting.Self;
using System;
using System.Configuration;

namespace ThumbsUp
{
	public class SelfHostingController
	{
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Url"];
		private NancyHost _nancyHost;

		public void Start()
		{
			Console.WriteLine("Starting the ThumbsUp Service on {0}", ThumbsUpsUrl);
			_nancyHost = new NancyHost(new Uri(ThumbsUpsUrl));
			_nancyHost.Start();
		}

		public void Stop()
		{
			_nancyHost.Stop();
			Console.WriteLine("The ThumbsUp Service has been successfully stopped.");
		}
	}
}
