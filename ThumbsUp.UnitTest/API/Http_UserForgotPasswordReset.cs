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
	public class Http_UserForgotPasswordReset : _BaseTest
	{
		[Fact]
		public void Should_return_new_password_when_valid_credentials_are_supplied()
		{
			// Given
			var passwordLength = PasswordService.PasswordCharactersCount;
			var token= MakeFake.Guid;

			var fakeUserService = MakeFake.UserService(new User());
			A.CallTo(() => fakeUserService.ForgotPasswordReset(A<User>.Ignored)).Returns(new string('*', passwordLength));

			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsForgotPasswordTokenValid(A<User>.Ignored, A<string>.Ignored)).Returns(true);

			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService: fakePasswordService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/reset", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<valid-username>");
				with.FormValue("token", token);
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
			TestMissingParams<UserModule>("/user/forgot-password/reset");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "username", MakeFake.Username }, { "token", MakeFake.InvalidGuid } };
			TestInvalidParams<UserModule>("/user/forgot-password/reset", formValues);
		}

		[Fact]
		public void Should_return_InvalidForgotPasswordToken_error_when_unknown_token_supplied()
		{
			// Given
			var token = MakeFake.Guid;
			var username = MakeFake.Username;
			var userToLoad = new User();
			var fakeUserService = MakeFake.UserService(userToLoad);
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsForgotPasswordTokenValid(A<User>.Ignored, A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService, fakePasswordService : fakePasswordService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/reset", with =>
			{
				with.HttpRequest();
				with.FormValue("username", username);
				with.FormValue("token", token);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.InvalidForgotPasswordToken);
		}

		#endregion
	}
}