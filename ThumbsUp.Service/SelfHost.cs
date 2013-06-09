
using Nancy.Helper;
using Nancy.Hosting.Self;
using System;
using System.Configuration;

namespace ThumbsUp
{
	public class SelfHost
	{
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Url"];
		private NancyHost nancyHost;

		public void Start()
		{
			Log.Write("Starting ThumbsUp on " + ThumbsUpsUrl);
			nancyHost = new NancyHost(new Uri(ThumbsUpsUrl));
			nancyHost.Start();
		}

		public void Stop()
		{
			Log.Write("Stopping ThumbsUp");
			nancyHost.Stop();
		}
	}
}
