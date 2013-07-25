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
using System.Configuration;

#endregion

namespace ThumbsUp.UnitTest.HttpAPI
{
	public class UserForgotPasswordReset : _BaseHttpTest
	{
		[Fact]
		public void Should_return_new_password_when_valid_credentials_are_supplied()
		{
			// Given
			var passwordLength = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]);
			var fakeToken = Guid.NewGuid().ToString();
			var fakeUserService = A.Fake<IUserService>();
			A.CallTo(() => fakeUserService.ForgotPasswordReset(A<string>.Ignored, A<string>.Ignored)).Returns(new string('*', passwordLength));
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/reset", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<username>");
				with.FormValue("token", fakeToken);
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
			var formValues = new Dictionary<string, string>() { { "username", "<valid-username>" }, { "token", "<invalid-token>" } };
			TestInvalidParams<UserModule>("/user/forgot-password/reset", formValues);
		}

		[Fact]
		public void Should_return_NoUserForCredentials_error_when_unknown_user_credentials_are_supplied()
		{
			// Given
			var fakeUserService = A.Fake<IUserService>();
			var fakeToken = Guid.NewGuid().ToString();
			A.CallTo(() => fakeUserService.ForgotPasswordReset(A<string>.Ignored, A<string>.Ignored)).Returns(null);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/reset", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<unknown-username>");
				with.FormValue("token", fakeToken);
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