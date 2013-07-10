#region Using

using Nancy;
using Nancy.Testing;
using Shouldly;
using System.Collections.Generic;
using System.Configuration;


#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class BaseTest
	{
		public readonly string ApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];

		public Browser NewBrowser()
		{
			var bootstrapper = new UnitTestBootstrapper();
			return new Browser(bootstrapper);
		}

		public void TestMissingParams(string apiEndpoint) 
		{
			// Given
			var browser = NewBrowser();

			// When
			var result = browser.Post(apiEndpoint, with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ErrorCode"].ToString().ShouldBe("2");
			payload["ErrorMessage"].ToString().ShouldBe("One or more required values were missing");
		}
	}
}