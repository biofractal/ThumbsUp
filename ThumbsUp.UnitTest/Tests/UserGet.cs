#region Using

using Nancy;
using Nancy.Helper;
using Nancy.Testing;
using Nancy.TinyIoc;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;
using FakeItEasy;

#endregion

namespace ThumbsUp.UnitTest.Tests
{		
	
	class UserGetBootstrapper:UnitTestBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			var cache = A.Fake<IUserCacheService>();
			A.CallTo(() => cache.Validate(A<string>.Ignored)).Returns(true);
			A.CallTo(() => cache.GetUser(A<string>.Ignored)).Returns(new User { UserName = "mock-user" });
			container.Register<IUserCacheService>(cache);
		}
	}

	public class UserGet: _BaseTest
	{		
		[Fact]
		public void Should_return_user_details_when_user_is_retrieved_with_valid_thumbkey()
		{
			// Given

			var browser = new Browser(new UserGetBootstrapper());
			var validThumbKey = Guid.NewGuid().ToString();

			// When
			var result = browser.Post("/user/get", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("thumbkey", validThumbKey);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			var user = payload["User"];
			user.ShouldNotBe(null);
			((Dictionary<string, object>)user)["UserName"].ShouldBe("mock-user");
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_user_is_retrieved_with_missing_params()
		{
			TestMissingParams("/user/get");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_user_is_retreived_with_invalid_thumbkey()
		{
			var formValues = new Dictionary<string, string>() { { "thumbkey", "<invalid>" } };
			TestInvalidParams("/user/get", formValues);
		}

		[Fact]
		public void Should_return_NoUserForThumbkey_error_when_user_is_retreived_with_valid_but_unrecognised_thumbkey()
		{
			// Given
			var browser = StdBrowser();

			// When
			var result = browser.Post("/user/get", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("thumbkey", Guid.NewGuid().ToString());
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ErrorCode"].ShouldBe(5);
			payload["ErrorMessage"].ShouldBe("The User could be found for the supplied ThumbKey");
		}	
		#endregion
	}
}