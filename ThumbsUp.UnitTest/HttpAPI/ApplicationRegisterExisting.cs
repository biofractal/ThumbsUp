#region Using

using Nancy;
using Nancy.Testing;
using Nancy.Helper;
using Shouldly;
using System;
using System.Collections.Generic;
using ThumbsUp.Service.Module;
using Xunit;
using ThumbsUp.Service;
using FakeItEasy;
using ThumbsUp.Service.Domain;

#endregion

namespace ThumbsUp.UnitTest.HttpAPI
{
	public class ApplicationRegisterExisting : _BaseHttpTest
	{
		[Fact]
		public void Should_return_existing_applicationid_when_existing_application_is_registered()
		{
			// Given
			var existingApplicationId = Guid.NewGuid().ToString();
			var fakeApplicationService = A.Fake<IApplicationService>();
			A.CallTo(() => fakeApplicationService.RegisterExisting(A<string>.Ignored, A<string>.Ignored)).Returns(new Application { Id = existingApplicationId });
			var applicationTestBrowser = MakeTestBrowser<ApplicationModule>(fakeApplicationService: fakeApplicationService);

			// When
			var result = applicationTestBrowser.Post("/application/register/existing", with =>
			{
				with.HttpRequest();
				with.FormValue("name", "Existing Application");
				with.FormValue("id", existingApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ApplicationId").ShouldBe(true);
			payload["ApplicationId"].ShouldBe(existingApplicationId);
		}

		#region Errors
		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<ApplicationModule>("/application/register/existing");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "name", "test"}, {"id", "<invalid>"} };
			TestInvalidParams<ApplicationModule>("/application/register/existing", formValues);
		}
		#endregion
	}
}