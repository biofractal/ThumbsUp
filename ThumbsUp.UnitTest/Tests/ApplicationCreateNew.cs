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
	public class ApplicationCreateNew : _BaseTest
	{
		[Fact]
		public void Should_return_new_applicationid_when_new_application_is_registered()
		{
			// Given
			var browser = StdBrowser();

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

		#region Errors
		[Fact]
		public void Should_return_MissingParameters_error_when_new_application_is_registered_with_missing_params()
		{
			TestMissingParams("/application/register/new");
		}
		#endregion
	}
}