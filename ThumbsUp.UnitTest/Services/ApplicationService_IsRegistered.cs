#region Using
using Shouldly;
using System;
using System.Configuration;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_IsRegistered : _BaseServiceTest
	{
		[Fact]
		public void Should_return_true_when_applicationid_is_known()
		{
			// Given
			var applicationId = Guid.NewGuid().ToString();
			var instanceToLoad = new Application();
			var fakeRavenSessionProvider = MakeFakeRavenSessionProvider<Application>(instanceToLoad);
			var applicationService = new ApplicationService(fakeRavenSessionProvider);

			// When
			var isRegistered = applicationService.IsRegistered(applicationId);

			// Then
			isRegistered.ShouldBe(true);
		}

		[Fact]
		public void Should_return_true_when_applicationid_is_builtin_adminid()
		{
			// Given
			var applicationId = ApplicationService.AdminId;
			var applicationService = new ApplicationService(DummyRavenSessionProvider);

			// When
			var isRegistered = applicationService.IsRegistered(applicationId);

			// Then
			isRegistered.ShouldBe(true);
		}

		[Fact]
		public void Should_return_false_when_applicationid_is_unknown()
		{
			// Given
			var applicationId = Guid.NewGuid().ToString();
			Application instanceToLoad = null;
			var fakeRavenSessionProvider = MakeFakeRavenSessionProvider<Application>(instanceToLoad);
			var applicationService = new ApplicationService(fakeRavenSessionProvider);

			// When
			var isRegistered = applicationService.IsRegistered(applicationId);

			// Then
			isRegistered.ShouldBe(false);
		}

		[Fact]
		public void Should_return_false_when_applicationid_is_missing()
		{
			// Given
			var applicationService = new ApplicationService(DummyRavenSessionProvider);

			// When
			var isRegistered = applicationService.IsRegistered(null);

			// Then
			isRegistered.ShouldBe(false);
		}

	}
}
