#region Using

using Nancy;
using Nancy.Helper;
using Nancy.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class ApplicationRegisterExisting : _BaseTest
	{
		[Fact]
		public void Should_return_existing_applicationid_when_existing_application_is_registered()
		{
			// Given
			var browser = StdBrowser();
			var existingApplicationId = Guid.NewGuid().ToString();

			// When
			var result = browser.Post("/application/register/existing", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("name", "Existing Application");
				with.FormValue("id", existingApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ApplicationId"].ShouldBe(existingApplicationId);
		}

		#region Errors
		[Fact]
		public void Should_return_MissingParameters_error_when_existing_application_is_registered_with_missing_params()
		{
			TestMissingParams("/application/register/existing");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_existing_application_is_registered_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "name", "test"}, {"id", "<invalid>"} };
			TestInvalidParams("/application/register/existing", formValues);
		}
		#endregion
	}
}