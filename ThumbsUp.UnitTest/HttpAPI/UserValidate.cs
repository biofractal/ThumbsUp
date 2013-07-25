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

namespace ThumbsUp.UnitTest.HttpAPI
{
	public class UserValidate : _BaseHttpTest
	{
		[Fact]
		public void Should_return_thumbkey_when_valid_user_credentials_are_supplied()
		{
			// Given
			var fakeUserService = A.Fake<IUserService>();
			A.CallTo(() => fakeUserService.ValidateUser(A<string>.Ignored, A<string>.Ignored)).Returns(new Guid().ToString());
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/validate", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<valid-username>");
				with.FormValue("password", "<valid-password>");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("ThumbKey").ShouldBe(true);
			payload["ThumbKey"].ToString().IsGuid().ShouldBe(true);
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
			var fakeUserService = A.Fake<IUserService>();
			A.CallTo(() => fakeUserService.ValidateUser(A<string>.Ignored, A<string>.Ignored)).Returns(null);
			var userTestBrowser = MakeTestBrowser<UserModule>(fakeUserService: fakeUserService);

			// When
			var result = userTestBrowser.Post("/user/validate", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<unknown-username>");
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