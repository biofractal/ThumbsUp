#region Using

using Nancy;
using Nancy.Testing;
using Nancy.Helper;
using Shouldly;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;


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

		public static Browser MakeTestBrowser<T>(IUserService mockUserService = null, IUserCacheService mockUserCacheService = null, IApplicationService mockApplicationService=null) where T : INancyModule
		{
			var bootstrapper = new ConfigurableBootstrapper(with =>
			{
				with.Module<T>();
				if (mockUserService == null) with.Dependency<IUserService>(typeof(UserService)); else with.Dependency<IUserService>(mockUserService);
				if (mockUserCacheService == null) with.Dependency<IUserCacheService>(typeof(UserCacheService)); else with.Dependency<IUserCacheService>(mockUserCacheService);
				if (mockApplicationService == null) with.Dependency<IApplicationService>(typeof(ApplicationService)); else with.Dependency<IApplicationService>(mockApplicationService);
				with.Dependency<IPasswordService>(typeof(PasswordService));
				with.Dependency<IRavenSessionProvider>(typeof(UnitTestRavenSessionProvider));
				with.Dependency<ICryptoService>(typeof(PBKDF2));
			});
			return new Browser(bootstrapper);
		}

		public void TestMissingParams<T>(string url, HttpMethod method = HttpMethod.POST) where T : INancyModule
		{
			// Then
			var result = TestParams<T>(url, method);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.MissingParameters);
		}

		public void TestInvalidParams<T>(string url, Dictionary<string, string> formValues, HttpMethod method = HttpMethod.POST) where T : INancyModule
		{
			// Then
			var result = TestParams<T>(url, method, formValues);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.InvalidParameters);
		}

		private BrowserResponse TestParams<T>(string url, HttpMethod method = HttpMethod.POST, Dictionary<string, string> formValues = null) where T : INancyModule
		{
			// Given
			var testBrowser = MakeTestBrowser<T>();
			var action = new Action<BrowserContext>(with =>
			{
				with.HttpRequest();
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
					return testBrowser.Get(url, action);
				case HttpMethod.POST:
				default:
					return testBrowser.Post(url, action);
			}
		}
	}
}