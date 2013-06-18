

using Nancy.Helper;
using Nancy.Hosting.Self;
using System;
using System.Configuration;

namespace ThumbsUp
{
	public class SelfHost
	{
		private static readonly string ServiceUrl = ConfigurationManager.AppSettings["Service.Url"];
		private NancyHost nancyHost;

		public void Start()
		{
			Log.Write("Starting on " + ServiceUrl);
			var config = new HostConfiguration
			{
				UnhandledExceptionCallback = e => Log.Error("Self Host Exception", e)
			};
			nancyHost = new NancyHost(config, new Uri(ServiceUrl));
			nancyHost.Start();
		}

		public void Stop()
		{
			Log.Write("Stopping");
			nancyHost.Stop();
		}
	}
}
