#region Using
using FakeItEasy;
using Raven.Helper;
using Shouldly;
using ThumbsUp.Domain;
using ThumbsUp.Service;
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
