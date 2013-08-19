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
using ThumbsUp.Domain;

using FakeItEasy;
using Raven.Client;


#endregion

namespace ThumbsUp.UnitTest
{
	public enum HttpMethod
	{
		GET,
		POST
	}

	public class _BaseTest
	{
		public static Browser MakeTestBrowser<T>(
			IUserService fakeUserService = null, 
			IUserCacheService fakeUserCacheService = null, 
			IApplicationService fakeApplicationService = null, 
			IPasswordService fakePasswordService = null, 
			ICryptoService fakeCryptoService = null, 
			IErrorService fakeErrorService = null) where T : INancyModule
		{
			var bootstrapper = new ConfigurableBootstrapper(with =>
			{
				with.Module<T>();
				with.Dependency<IRavenSessionProvider>(A.Dummy<IRavenSessionProvider>());

				if (fakeUserService == null) with.Dependency<IUserService>(A.Dummy<IUserService>()); else with.Dependency<IUserService>(fakeUserService);
				if (fakeUserCacheService == null) with.Dependency<IUserCacheService>(A.Dummy<IUserCacheService>()); else with.Dependency<IUserCacheService>(fakeUserCacheService);
				if (fakeApplicationService == null) with.Dependency<IApplicationService>(A.Dummy<IApplicationService>()); else with.Dependency<IApplicationService>(fakeApplicationService);
				if (fakePasswordService == null) with.Dependency<IPasswordService>(A.Dummy<IPasswordService>()); else with.Dependency<IPasswordService>(fakePasswordService);
				if (fakeCryptoService == null) with.Dependency<ICryptoService>(A.Dummy<ICryptoService>()); else with.Dependency<ICryptoService>(fakeCryptoService);
				if (fakeErrorService == null) with.Dependency<IErrorService>(A.Dummy<IErrorService>()); else with.Dependency<IErrorService>(fakeErrorService);
			});
			return new Browser(bootstrapper);
		}

		public static Browser MakeTestErrorBrowser<T>(
			IUserService fakeUserService = null,
			IUserCacheService fakeUserCacheService = null,
			IApplicationService fakeApplicationService = null,
			IPasswordService fakePasswordService = null,
			ICryptoService fakeCryptoService = null) where T : INancyModule
		{
			var errorService = new ErrorService();
			return MakeTestBrowser<T>(fakeUserService, fakeUserCacheService, fakeApplicationService, fakePasswordService, fakeCryptoService, errorService);
		}

		public void TestMissingParams<T>(string url, HttpMethod method = HttpMethod.POST, Browser browser = null) where T : INancyModule
		{
			// Then
			var result = TestParams<T>(url, method, browser: browser);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.MissingParameters);
		}

		public void TestInvalidParams<T>(string url, Dictionary<string, string> formValues, HttpMethod method = HttpMethod.POST, Browser browser = null) where T : INancyModule
		{
			// Then
			var result = TestParams<T>(url, method, formValues, browser);
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.InvalidParameters);
		}

		private BrowserResponse TestParams<T>(string url, HttpMethod method = HttpMethod.POST, Dictionary<string, string> formValues = null, Browser browser = null) where T : INancyModule
		{
			// Given
			var testBrowser = browser ?? MakeTestErrorBrowser<T>();
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