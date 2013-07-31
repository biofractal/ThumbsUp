#region Using
using FakeItEasy;
using Nancy.Helper;
using Shouldly;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Raven;
using Xunit;
using Xunit.Extensions;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_RegisterExisting : _BaseTest
	{
		[Fact]
		public void Should_return_new_Application_when_application_is_registered_with_existing_applicationid()
		{
			// Given
			var applicationService = new ApplicationService(DummyRavenSessionProvider);
			var applicationName = "<application-name>";
			var applicationId = Guid.NewGuid().ToString();

			// When
			var application = applicationService.RegisterExisting(applicationName, applicationId);

			// Then
			application.Name.ShouldBe(applicationName);
			application.Id.ShouldBe(applicationId);
		}

		[
			Theory,
			InlineData("", ValidGuid),
			InlineData("<application-name>", ""),
			InlineData("", ""),
			InlineData("<application-name>", InvalidGuid)
		]
		public void Should_return_null_when_parameters_are_missing_or_invalid(string applicationName, string applicationId)
		{
			// Given
			var applicationService = new ApplicationService(DummyRavenSessionProvider);

			// When
			var application = applicationService.RegisterExisting(applicationName, applicationId);

			// Then
			application.ShouldBe(null);
		}

	}
}
