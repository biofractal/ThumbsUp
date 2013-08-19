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
using System.Configuration;

#endregion

namespace ThumbsUp.UnitTest.API
{
	public class Http_UserResetPassword : _BaseTest
	{
		[Fact]
		public void Should_return_new_password_when_valid_credentials_are_supplied()
		{
			// Given
			var passwordLength = PasswordService.PasswordCharactersCount;
			var fakeUserService = MakeFake.UserService(new User());
			A.CallTo(() => fakeUserService.ResetPassword(A<User>.Ignored)).Returns(new string('*', passwordLength));
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsPasswordValid(A<User>.Ignored, A<string>.Ignored)).Returns(true);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService: fakePasswordService);

			// When
			var result = userTestBrowser.Post("/user/reset/password", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<username>");
				with.FormValue("password", "<password>");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("Password").ShouldBe(true);
			payload["Password"].ToString().Length.ShouldBe(passwordLength);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/reset/password");
		}

		[Fact]
		public void Should_return_NoUserForCredentials_error_when_unknown_username_is_supplied()
		{
			// Given
			var fakeUserService = MakeFake.UserService(new User());
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/reset/password", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<unknown-username>");
				with.FormValue("password", "<valid-password>");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForCredentials);
		}

		[Fact]
		public void Should_return_NoUserForCredentials_error_when_unknown_password_is_supplied()
		{
			// Given

			var fakeUserService = MakeFake.UserService(new User());
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsPasswordValid(A<User>.Ignored, A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService: fakePasswordService);

			// When
			var result = userTestBrowser.Post("/user/reset/password", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<valid-username>");
				with.FormValue("password", "<unknown-password>");
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