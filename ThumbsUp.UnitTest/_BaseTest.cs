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
		public readonly IRavenSessionProvider DummyRavenSessionProvider = A.Dummy<IRavenSessionProvider>();
		public const string ValidGuid = "0e2ae555-9fc7-4b89-8ea4-a8b63097c50a";
		public const string InvalidGuid = "<invalid-guid>";


		public const string ValidHash = "W5OS/v3YxIEYRvMtYc7TYDFbbvucPaqwuThGe2wbXlVkmfkHh7InpXfcw0V8ansVDdqA2zhAooKkVKIjv3x14g==";
		public const string ValidSalt = "100000.4n9+TkbFFA7Do1QBbHgkWb4dM5WL5iLJLgpwEarnJnNoNw==";
		public const string ValidPassword = "2Qr}b8_N$yZ6";
		public const string ValidUsername = "<valid-username>";
		public const string ValidEmail = "valid@email.com";
		public const string InvalidEmail = "<invalid-email>";

		public IRavenSessionProvider MakeFakeRavenSessionProvider<T>(T instanceToLoad)
		{
			var fakeDocumentSession = A.Fake<IDocumentSession>();
			A.CallTo(() => fakeDocumentSession.Load<T>(A<string>.Ignored)).Returns(instanceToLoad);
			var fakeRavenSessionProvider = A.Fake<IRavenSessionProvider>();
			A.CallTo(() => fakeRavenSessionProvider.Get()).Returns(fakeDocumentSession);
			return fakeRavenSessionProvider;
		}

		public IUserService MakeFakeUserService(User userToReturn=null)
		{
			var fakeUserService = A.Fake<IUserService>();
			A.CallTo(() => fakeUserService.GetUserByName(A<string>.Ignored)).Returns(userToReturn);
			return fakeUserService;
		}

		public IPasswordService MakeFakePasswordService()
		{
			var fakePassword = A.Fake<IPassword>();
			A.CallTo(() => fakePassword.Clear).Returns(ValidPassword);
			var fakePasswordService = A.Fake<IPasswordService>();
			A.CallTo(() => fakePasswordService.Generate()).Returns(fakePassword);
			return fakePasswordService;
		}

		public static Browser MakeTestBrowser<T>(IUserService fakeUserService = null, IUserCacheService fakeUserCacheService = null, IApplicationService fakeApplicationService = null, IPasswordService fakePasswordService = null) where T : INancyModule
		{
			var bootstrapper = new ConfigurableBootstrapper(with =>
			{
				with.Module<T>();
				if (fakeUserService == null) with.Dependency<IUserService>(typeof(UserService)); else with.Dependency<IUserService>(fakeUserService);
				if (fakeUserCacheService == null) with.Dependency<IUserCacheService>(typeof(UserCacheService)); else with.Dependency<IUserCacheService>(fakeUserCacheService);
				if (fakeApplicationService == null) with.Dependency<IApplicationService>(typeof(ApplicationService)); else with.Dependency<IApplicationService>(fakeApplicationService);
				if (fakePasswordService == null) with.Dependency<IPasswordService>(typeof(PasswordService)); else with.Dependency<IPasswordService>(fakePasswordService);
				with.Dependency<IRavenSessionProvider>(A.Dummy<IRavenSessionProvider>());
				with.Dependency<ICryptoService>(typeof(PBKDF2));
				with.Dependency<IErrorService>(typeof(ErrorService));
			});
			return new Browser(bootstrapper);
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
			var testBrowser = browser ?? MakeTestBrowser<T>();
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