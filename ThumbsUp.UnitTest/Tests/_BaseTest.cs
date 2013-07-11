#region Using

using Nancy;
using Nancy.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;


#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public enum HttpMethod
	{
		GET,
		POST
	}
	public class _BaseTest
	{

		public readonly string ApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];

		public Browser StdBrowser()
		{
			var bootstrapper = new UnitTestBootstrapper();
			return new Browser(bootstrapper);
		}

		public void TestMissingParams(string url, HttpMethod method = HttpMethod.POST)
		{
			// Then
			var result = TestParams(url, method);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ErrorCode"].ShouldBe(2);
			payload["ErrorMessage"].ShouldBe("One or more required values were missing");
		}

		public void TestInvalidParams(string url, Dictionary<string, string> formValues, HttpMethod method = HttpMethod.POST)
		{
			// Then
			var result = TestParams(url, method, formValues);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ErrorCode"].ShouldBe(1);
			payload["ErrorMessage"].ShouldBe("One or more required values were invalid");
		}

		private BrowserResponse TestParams(string url, HttpMethod method = HttpMethod.POST, Dictionary<string, string> formValues = null)
		{
			// Given
			var browser = StdBrowser();
			var action = new Action<BrowserContext>(with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				if (formValues == null) return;
				foreach (var param in formValues)
				{
					with.FormValue(param.Key, param.Value);
				}
			});

			// When
			switch (method)
			{
				case HttpMethod.GET:
					return browser.Get(url, action);
				case HttpMethod.POST:
				default:
					return browser.Post(url, action);
			}
		}
	}
}