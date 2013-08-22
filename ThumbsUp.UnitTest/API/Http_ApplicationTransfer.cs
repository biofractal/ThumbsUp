#region Using

using FakeItEasy;
using Nancy;
using Nancy.Helper;
using Nancy.Testing;
using Shouldly;
using System.Collections.Generic;
using ThumbsUp.Domain;
using ThumbsUp.Service.Module;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.API
{
	public class Http_ApplicationTransfer : _BaseTest
	{
		[Fact]
		public void Should_return_existing_applicationid_when_existing_application_is_registered()
		{
			// Given
			var singleUseToken = MakeFake.Guid;
			var existingApplicationId = MakeFake.Guid;
			var fakeApplicationService = MakeFake.ApplicationService();
			A.CallTo(() => fakeApplicationService.AuthoriseSingleUseToken(A<string>.Ignored)).Returns(true);
			A.CallTo(() => fakeApplicationService.Transfer(A<string>.Ignored, A<string>.Ignored)).Returns(new Application { Id = existingApplicationId });
			var applicationTestBrowser = MakeTestBrowser<ApplicationModule>(fakeApplicationService: fakeApplicationService);

			// When
			var result = applicationTestBrowser.Post("/application/transfer", with =>
			{
				with.HttpRequest();
				with.FormValue("singleUseToken", singleUseToken);
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
			TestMissingParams<ApplicationModule>("/application/transfer");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { {"singleUseToken", MakeFake.Guid}, { "name", MakeFake.Name}, {"id", MakeFake.InvalidGuid} };
			TestInvalidParams<ApplicationModule>("/application/transfer", formValues);
		}
		#endregion
	}
}