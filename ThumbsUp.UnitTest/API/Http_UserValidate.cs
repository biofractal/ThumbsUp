#region Using

using FakeItEasy;
using Nancy;
using Nancy.Testing;
using Nancy.Helper;
using Shouldly;
using System;
using System.Collections.Generic;
using ThumbsUp.Service;
using ThumbsUp.Domain;
using ThumbsUp.Service.Module;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.API
{
	public class Http_UserValidate : _BaseTest
	{
		[Fact]
		public void Should_return_thumbkey_when_valid_user_credentials_are_supplied()
		{
			// Given
			var username = MakeFake.Username;
			var password = MakeFake.Password;
			var thumbKey = MakeFake.Guid;
			User userToLoad = new User();
			var fakeUserService = MakeFake.UserService(userToLoad);
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsPasswordValid(A<User>.Ignored, A<string>.Ignored)).Returns(true);
			var fakeUserCacheService = A.Fake<IUserCacheService>();
			A.CallTo(() => fakeUserCacheService.Add(A<User>.Ignored)).Returns(thumbKey);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService: fakePasswordService, fakeUserCacheService: fakeUserCacheService);

			// When
			var result = userTestBrowser.Post("/user/validate", with =>
			{
				with.HttpRequest();
				with.FormValue("username", username);
				with.FormValue("password", password);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ThumbKey").ShouldBe(true);
			payload["ThumbKey"].ShouldBe(thumbKey);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/validate");
		}

		[Fact]
		public void Should_return_NoUserForCredentials_error_when_unknown_user_credentials_are_supplied()
		{
			// Given
			var username = MakeFake.Username;
			var password = MakeFake.Password;
			User userToLoad = null;
			var fakeUserService = MakeFake.UserService(userToLoad);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/validate", with =>
			{
				with.HttpRequest();
				with.FormValue("username", username);
				with.FormValue("password", password);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForCredentials);
		}


		[Fact]
		public void Should_return_NoUserForCredentials_error_when_user_password_does_not_match_password_supplied()
		{
			// Given
			var username = MakeFake.Username;
			var password = MakeFake.Password;
			User userToLoad = new User();
			var fakeUserService = MakeFake.UserService(userToLoad);
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsPasswordValid(A<User>.Ignored, A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService: fakePasswordService);

			// When
			var result = userTestBrowser.Post("/user/validate", with =>
			{
				with.HttpRequest();
				with.FormValue("username", username);
				with.FormValue("password", password);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForCredentials);
		}
		#endregion
	}
}