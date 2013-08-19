#region Using
using FakeItEasy;
using Nancy.Helper;
using Shouldly;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;
using Xunit;
using Xunit.Extensions;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ApplicationService_Transfer
	{
		[Fact]
		public void Should_return_new_Application_when_application_is_registered_with_existing_applicationid()
		{
			// Given
			var applicationName = MakeFake.Name;
			var applicationId = MakeFake.Guid;
			var instanceToLoad = new Application() { Id = applicationId, Name = applicationName };
			var fakeRavenSessionProvider = MakeFake.RavenSessionProvider<Application>(instanceToLoad);
			var applicationService = new ApplicationService(fakeRavenSessionProvider);

			// When
			var application = applicationService.Transfer(applicationName, applicationId);

			// Then
			application.Name.ShouldBe(applicationName);
			application.Id.ShouldBe(applicationId);
		}

		[
			Theory,
			InlineData("", MakeFake.Guid),
			InlineData(MakeFake.Name, ""),
			InlineData("", ""),
			InlineData(MakeFake.Name, MakeFake.InvalidGuid)
		]
		public void Should_return_null_when_parameters_are_missing_or_invalid(string applicationName, string applicationId)
		{
			// Given
			var applicationService = new ApplicationService(A.Dummy<IRavenSessionProvider>());

			// When
			var application = applicationService.Transfer(applicationName, applicationId);

			// Then
			application.ShouldBe(null);
		}

	}
}
