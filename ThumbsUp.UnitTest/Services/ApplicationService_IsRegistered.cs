#region Using
using FakeItEasy;
using Shouldly;
using System;
using System.Configuration;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_IsRegistered
	{
		[Fact]
		public void Should_return_true_when_applicationid_is_known()
		{
			// Given
			var applicationId = MakeFake.Guid;
			var instanceToLoad = new Application();
			var fakeRavenSessionProvider = MakeFake.RavenSessionProvider<Application>(instanceToLoad);
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
			var applicationService = new ApplicationService(A.Dummy<IRavenSessionProvider>());

			// When
			var isRegistered = applicationService.IsRegistered(applicationId);

			// Then
			isRegistered.ShouldBe(true);
		}

		[Fact]
		public void Should_return_false_when_applicationid_is_unknown()
		{
			// Given
			var applicationId = MakeFake.Guid;
			Application instanceToLoad = null;
			var fakeRavenSessionProvider = MakeFake.RavenSessionProvider<Application>(instanceToLoad);
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
			string applicationId = null;
			var applicationService = new ApplicationService(A.Dummy<IRavenSessionProvider>());

			// When
			var isRegistered = applicationService.IsRegistered(applicationId);

			// Then
			isRegistered.ShouldBe(false);
		}

	}
}
