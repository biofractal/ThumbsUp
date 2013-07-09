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
	public class ApplicationTests : BaseTest
	{

		[Fact]
		public void Should_return_new_applicationid_when_new_application_is_registered()
		{
			// Given
			var browser = NewBrowser();

			// When
			var result = browser.Post("/application/register/new", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("name", "New Application");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ApplicationId"].ToString().IsGuid().ShouldBe(true);
		}

		[Fact]
		public void Should_return_existing_applicationid_when_existing_application_is_registered()
		{
			// Given
			var browser = NewBrowser();
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

	}
}