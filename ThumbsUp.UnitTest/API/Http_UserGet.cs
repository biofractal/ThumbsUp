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
	public class Http_UserGet : _BaseTest
	{
		[Fact]
		public void Should_return_user_details_when_user_is_retrieved_with_valid_thumbkey()
		{
			// Given
			var thumbkey = MakeFake.Guid;
			var username = MakeFake.Username;
			var userToLoad = new User { UserName = username};
			var fakeUserCacheService = MakeFake.UserCacheService(userToLoad);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserCacheService: fakeUserCacheService);
			
			// When
			var result = userTestBrowser.Post("/user/get", with =>
			{
				with.HttpRequest();
				with.FormValue("thumbkey", thumbkey);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("User").ShouldBe(true);
			var user = payload["User"];
			((Dictionary<string, object>)user)["UserName"].ShouldBe(username);
		}


		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/get");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "thumbkey", MakeFake.InvalidGuid } };
			TestInvalidParams<UserModule>("/user/get", formValues);
		}

		[Fact]
		public void Should_return_NoUserForThumbkey_error_when_user_is_retrieved_with_valid_but_unknown_thumbkey()
		{
			// Given
			var unknownThumbkey = MakeFake.Guid;
			User userToLoad = null;
			var fakeUserCacheService = MakeFake.UserCacheService(userToLoad);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserCacheService: fakeUserCacheService);

			// When
			var result = userTestBrowser.Post("/user/get", with =>
			{
				with.HttpRequest();
				with.FormValue("thumbkey", unknownThumbkey);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForThumbkey);
		}
		#endregion
	}
}