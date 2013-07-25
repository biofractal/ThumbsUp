#region Using
using Shouldly;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_Get : _BaseServiceTest
	{
		[Fact]
		public void Should_return_application_when_known_applicationid_is_supplied()
		{
			// Given
			var applicationId = Guid.NewGuid().ToString();
			var applicationName = "<application-name>";
			var instanceToLoad = new Application() { Id = applicationId, Name = applicationName };
			var fakeRavenSessionProvider = MakeFakeRavenSessionProvider<Application>(instanceToLoad);
			var applicationService = new ApplicationService(fakeRavenSessionProvider);

			// When
			var application = applicationService.Get(applicationId);

			// Then
			application.Name.ShouldBe(applicationName);
			application.Id.ShouldBe(applicationId);
		}
	}
}
