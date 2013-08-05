#region Using

using FakeItEasy;
using Nancy;
using Nancy.Testing;
using Nancy.Helper;
using Shouldly;
using System;
using System.Collections.Generic;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Module;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.API
{
	public class Http_UserValidateName : _BaseTest
	{
		[Fact]
		public void Should_return_OK_when_unused_username_is_supplied()
		{
			// Given
			User userToLoad = null;
			var fakeUserService = MakeFake.UserService(userToLoad);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/validate/name", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<valid-username>");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/validate/name");
		}

		[Fact]
		public void Should_return_UserNameTaken_error_when_existing_username_is_supplied()
		{
			// Given		
			User userToLoad = new User();
			var fakeUserService = MakeFake.UserService(userToLoad);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/validate/name", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<existing-username>");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.UserNameTaken);
		}
		#endregion
	}
}