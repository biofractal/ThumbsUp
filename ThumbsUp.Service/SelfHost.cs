

using Nancy.Helper;
using Nancy.Hosting.Self;
using System;
using System.Configuration;
using System.Threading;

namespace ThumbsUp.Service
{
	public class SelfHost
	{
		//Prevent more than one instance running
		private static Mutex mutex = new Mutex(true, @"Global\{9b41dbe3-4e1a-4b6b-aa5a-ded9c7ef29f9}");
		private static readonly string ServiceUrl = ConfigurationManager.AppSettings["Service.Url"];
		private NancyHost nancyHost;

		public void Start()
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				try
				{
					Log.Write("Starting on " + ServiceUrl);
					var config = new HostConfiguration
					{
						UnhandledExceptionCallback = e => Log.Error("Self Host Exception", e)
					};
					nancyHost = new NancyHost(config, new Uri(ServiceUrl));
					nancyHost.Start();
				}
				catch (Exception ex)
				{
					Log.Error("Starting ThumbsUp", ex);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			else
			{
				Log.Write("Cannot start multiple instances. The ThumbsUp Service is already running on " + ServiceUrl);
				Environment.Exit(0);
			}
		}

		public void Stop()
		{
			try
			{
				Log.Write("Stopping");
				nancyHost.Stop();
			}
			catch (Exception ex)
			{
				Log.Error("Stopping ThumbsUp", ex);
			}
			finally
			{
				mutex.Close();
			}
		}
	}
}
