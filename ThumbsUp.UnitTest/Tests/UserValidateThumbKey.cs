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

namespace ThumbsUp.UnitTest.Tests
{
	public class UserValidateThumbKey : _BaseTest
	{
		[Fact]
		public void Should_return_OK_when_valid_thumbkey_is_supplied()
		{
			// Given
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.ValidateIdentifier(A<string>.Ignored)).Returns(true);
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);
			var validThumbkey = Guid.NewGuid().ToString();

			// When
			var result = userTestBrowser.Post("/user/validate/thumbkey", with =>
			{
				with.HttpRequest();
				with.FormValue("thumbkey", validThumbkey);
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_user_is_retrieved_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/validate/thumbkey");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_user_is_retrieved_with_invalid_thumbkey()
		{
			var formValues = new Dictionary<string, string>() { { "thumbkey", "<invalid-guid>" } };
			TestInvalidParams<UserModule>("/user/validate/thumbkey", formValues);
		}

		[Fact]
		public void Should_return_NoUserForThumbkey_error_when_valid_but_unknown_thumbkey_is_supplied()
		{
			// Given
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.ValidateIdentifier(A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);
			var unknownThumbkey = Guid.NewGuid().ToString();

			// When
			var result = userTestBrowser.Post("/user/validate/thumbkey", with =>
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