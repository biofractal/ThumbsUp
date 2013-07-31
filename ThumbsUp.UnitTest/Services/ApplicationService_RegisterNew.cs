#region Using
using FakeItEasy;
using Nancy.Helper;
using Shouldly;
using ThumbsUp.Service;
using ThumbsUp.Service.Raven;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_RegisterNew : _BaseTest
	{
		[Fact]
		public void Should_return_new_Application_when_new_application_is_registered()
		{
			// Given
			var applicationService = new ApplicationService(DummyRavenSessionProvider);
			var applicationName = "<application-name>";

			// When
			var application = applicationService.RegisterNew(applicationName);

			// Then
			application.Name.ShouldBe(applicationName);
			application.Id.IsGuid().ShouldBe(true);
		}

		[Fact]
		public void Should_return_null_when_application_name_is_missing()
		{
			// Given
			var applicationService = new ApplicationService(DummyRavenSessionProvider);

			// When
			var application = applicationService.RegisterNew(string.Empty);

			// Then
			application.ShouldBe(null);
		}
	}
}
