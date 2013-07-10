#region Using

using System;
using System.IO;
using Nancy;
using Nancy.Testing.Fakes;
using Nancy.TinyIoc;
using ThumbsUp.Service.Raven;
using ThumbsUp.UnitTest;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service;
using Nancy.Bootstrapper;
using SimpleCrypto;

#endregion

namespace ThumbsUp.UnitTest
{
	public class UnitTestBootstrapper : ThumbsUpBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			container.Register<IRavenSessionProvider, UnitTestRavenSessionProvider>().AsSingleton();
		}
	}
}