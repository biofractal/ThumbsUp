#region Using

using Nancy;
using Nancy.Testing;
using Shouldly;
using System;
using System.Configuration;
using System.Diagnostics;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class Root : _BaseTest
	{

		[Fact]
		public void Should_return_status_ok_when_applicationid_is_builtin_adminid()
		{
			// Given
			var browser = StdBrowser();

			// When
			var result = browser.Get("/", with =>
			{
				with.HttpRequest();
				with.Query("applicationid", ApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
			result.Body.AsString().ShouldBe("ThumbsUp Security Service is Running");
		}

		#region Errors
		[Fact]
		public void Should_return_status_unauthorized_when_applicationid_is_missing()
		{
			// Given
			var browser = StdBrowser();

			// When
			var result = browser.Get("/", with =>
			{
				with.HttpRequest();
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public void Should_return_status_unauthorized_when_applicationid_is_not_registered()
		{
			// Given
			var browser = StdBrowser();

			// When
			var result = browser.Get("/", with =>
			{
				with.HttpRequest();
				with.Query("applicationid", Guid.NewGuid().ToString());
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
		#endregion
	}
}