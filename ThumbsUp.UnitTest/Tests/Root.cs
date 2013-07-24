#region Using

using Nancy;
using Nancy.Testing;
using Nancy.TinyIoc;
using Shouldly;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Module;
using ThumbsUp.Service.Raven;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class Root : _BaseTest
	{
		private class RootBootstrapper : ThumbsUpBootstrapper
		{
			protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
			{
				base.ConfigureRequestContainer(container, context);
				container.Register<IRavenSessionProvider, UnitTestRavenSessionProvider>().AsSingleton();
			}
		}

		// Given
		private readonly Browser RootTestBrowser = new Browser(new RootBootstrapper());

		[Fact]
		public void Should_return_status_ok_when_applicationid_is_builtin_adminid()
		{

			var builtInApplicationId = "ac8dfe73-4cc2-409c-99bc-36e738f6e29c";

			// When
			var result = RootTestBrowser.Get("/", with =>
			{
				with.HttpRequest();
				with.Query("applicationid", builtInApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
			result.Body.AsString().ShouldBe("ThumbsUp Security Service is Running");
		}

		#region Errors
		[Fact]
		public void Should_return_status_unauthorized_when_applicationid_is_missing()
		{
			// When
			var result = RootTestBrowser.Get("/", with =>
			{
				with.HttpRequest();
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public void Should_return_status_unauthorized_when_applicationid_is_not_registered()
		{
			// Given
			var unregisteredApplicationId = Guid.NewGuid().ToString();

			// When
			var result = RootTestBrowser.Get("/", with =>
			{
				with.HttpRequest();
				with.Query("applicationid", unregisteredApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
		#endregion
	}
}