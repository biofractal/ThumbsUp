#region Using
using Shouldly;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_Get
	{
		[Fact]
		public void Should_return_application_when_known_applicationid_is_supplied()
		{
			// Given
			var applicationId = MakeFake.Guid;
			var applicationName = MakeFake.Name;
			var instanceToLoad = new Application() { Id = applicationId, Name = applicationName };
			var fakeRavenSessionProvider = MakeFake.RavenSessionProvider<Application>(instanceToLoad);
			var applicationService = new ApplicationService(fakeRavenSessionProvider);

			// When
			var application = applicationService.Get(applicationId);

			// Then
			application.Name.ShouldBe(applicationName);
			application.Id.ShouldBe(applicationId);
		}
	}
}
