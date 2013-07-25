#region Using

using FakeItEasy;
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

namespace ThumbsUp.UnitTest.HttpAPI
{
	public class Root : _BaseHttpTest
	{
		private class RootBootstrapper : ThumbsUpBootstrapper
		{
			private IApplicationService FakeApplicationService;

			public RootBootstrapper(IApplicationService fakeApplicationService) 
			{
				FakeApplicationService = fakeApplicationService;
			}

			protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
			{
				base.ConfigureRequestContainer(container, context);
				container.Register<IApplicationService>(FakeApplicationService);
				container.Register<IRavenSessionProvider>(A.Dummy<IRavenSessionProvider>());
			}
		}


		[Fact]
		public void Should_return_OK_when_applicationid_is_registered()
		{
			// Given				
			var fakeApplicationService = A.Fake<IApplicationService>();
			A.CallTo(() => fakeApplicationService.IsRegistered(A<string>.Ignored)).Returns(true);
			var rootTestBrowser = new Browser(new RootBootstrapper(fakeApplicationService));
			var registeredApplicationId = Guid.NewGuid().ToString();

			var validApplicationId = Guid.NewGuid().ToString();

			// When
			var result = rootTestBrowser.Get("/", with =>
			{
				with.HttpRequest();
				with.Query("applicationid", registeredApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		#region Errors
		[Fact]
		public void Should_return_unauthorized_when_applicationid_is_missing()
		{
			// Given				
			var fakeApplicationService = A.Fake<IApplicationService>();
			A.CallTo(() => fakeApplicationService.IsRegistered(A<string>.Ignored)).Returns(false);
			var rootTestBrowser = new Browser(new RootBootstrapper(fakeApplicationService));

			// When
			var result = rootTestBrowser.Get("/", with =>
			{
				with.HttpRequest();
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public void Should_return_unauthorized_when_applicationid_is_not_registered()
		{
			// Given				
			var fakeApplicationService = A.Fake<IApplicationService>();
			A.CallTo(() => fakeApplicationService.IsRegistered(A<string>.Ignored)).Returns(false);
			var rootTestBrowser = new Browser(new RootBootstrapper(fakeApplicationService));
			var unregisteredApplicationId = Guid.NewGuid().ToString();

			// When
			var result = rootTestBrowser.Get("/", with =>
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