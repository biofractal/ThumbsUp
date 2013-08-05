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
	public class Http_UserLogout : _BaseTest
	{
		[Fact]
		public void Should_return_OK_when_valid_thumbkey_is_supplied()
		{
			// Given
			var fakeUserCacheService = MakeFake.UserCacheService();
			A.CallTo(() => fakeUserCacheService.Remove(A<string>.Ignored)).Returns(true);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserCacheService: fakeUserCacheService);
			var validGuid = MakeFake.Guid;

			// When
			var result = userTestBrowser.Post("/user/logout", with =>
			{
				with.HttpRequest();
				with.FormValue("thumbkey", validGuid);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/logout");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "thumbkey", MakeFake.InvalidGuid } };
			TestInvalidParams<UserModule>("/user/logout", formValues);
		}

		[Fact]
		public void Should_return_NoUserForThumbkey_error_when_user_is_retrieved_with_valid_but_unknown_thumbkey()
		{
			// Given
			var fakeUserCacheService = MakeFake.UserCacheService();
			A.CallTo(() => fakeUserCacheService.Validate(A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserCacheService: fakeUserCacheService);
			var unknownThumbkey = MakeFake.Guid;

			// When
			var result = userTestBrowser.Post("/user/logout", with =>
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