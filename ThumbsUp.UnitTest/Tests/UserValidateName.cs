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
	public class UserValidateName : _BaseTest
	{
		[Fact]
		public void Should_return_OK_when_unused_username_is_supplied()
		{
			// Given
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.IsValidUserName(A<string>.Ignored)).Returns(true);
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);

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
		public void Should_return_MissingParameters_error_when_user_is_retrieved_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/validate/name");
		}

		[Fact]
		public void Should_return_UserNameTaken_error_when_existing_username_is_supplied()
		{
			// Given
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.IsValidUserName(A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);

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