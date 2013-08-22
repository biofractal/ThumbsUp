#region Using

using FakeItEasy;
using Nancy;
using Nancy.Helper;
using Nancy.Testing;
using Shouldly;
using System.Collections.Generic;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using ThumbsUp.Service.Module;
using Xunit;


#endregion

namespace ThumbsUp.UnitTest.API
{
	public class Http_UserForgotPasswordRequest : _BaseTest
	{
		[Fact]
		public void Should_return_new_password_when_valid_credentials_are_supplied()
		{
			// Given
			var fakeToken = MakeFake.Guid;
			var email = MakeFake.Email;
			var fakeUserService = MakeFake.UserService(new User() { Email=email});
			A.CallTo(() => fakeUserService.ForgotPasswordRequest(A<User>.Ignored)).Returns(fakeToken);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/request", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<username>");
				with.FormValue("email", email);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("Token").ShouldBe(true);
			payload["Token"].ShouldBe(fakeToken);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_endpoint_is_hit_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/forgot-password/request");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_endpoint_is_hit_with_invalid_params()
		{
			var formValues = new Dictionary<string, string>() { { "username", MakeFake.Username }, { "email", MakeFake.InvalidEmail } };
			TestInvalidParams<UserModule>("/user/forgot-password/request", formValues);
		}

		[Fact]
		public void Should_return_NoUserForCredentials_error_when_unknown_username_is_supplied()
		{
			// Given
			var fakeUserService = MakeFake.UserService();
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/request", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<unknown-username>");
				with.FormValue("email", "valid@email.com");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForCredentials);
		}

		[Fact]
		public void Should_return_NoUserForEmail_error_when_unknown_email_is_supplied()
		{
			// Given
			var email = MakeFake.Email;
			var fakeUserService = MakeFake.UserService(new User() { Email=email});
			var userTestBrowser = MakeTestErrorBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/forgot-password/request", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<valid-username>");
				with.FormValue("email", "unknown@email.com");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ErrorCode", "ErrorMessage").ShouldBe(true);
			payload["ErrorCode"].ShouldBe((int)ErrorCode.NoUserForEmail);
		}

		#endregion
	}
}