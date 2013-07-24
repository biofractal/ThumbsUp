#region Using

using FakeItEasy;
using Nancy;
using Nancy.Testing;
using Nancy.Helper;
using Shouldly;
using System.Collections.Generic;
using System.Configuration;
using ThumbsUp.Service;
using ThumbsUp.Service.Module;
using Xunit;
using ThumbsUp.Service.Domain;

#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class UserCreate : _BaseTest
	{
		[Fact]
		public void Should_return_password_when_user_is_created()
		{
			// Given
			var passwordLength = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]);
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.IsValidUserName(A<string>.Ignored)).Returns(true);
			A.CallTo(() => mockUserService.CreateUser(A<string>.Ignored, A<string>.Ignored)).Returns(new string('*', passwordLength));
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);

			// When
			var result = userTestBrowser.Post("/user/create", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<username>");
				with.FormValue("email", "valid@email.com");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload.ContainsItems("Password").ShouldBe(true);
			payload["Password"].ToString().Length.ShouldBe(passwordLength);
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_user_is_created_with_missing_params()
		{
			TestMissingParams<UserModule>("/user/create");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_user_is_created_with_invalid_email()
		{	
			var formValues = new Dictionary<string, string>() { { "username", "<username>" }, { "email", "<invalid-email>" } };
			TestInvalidParams<UserModule>("/user/create", formValues);
		}

		[Fact]
		public void Should_return_UserNameTaken_error_when_user_is_created_with_existing_username()
		{
			// Given
			var mockUserService = A.Fake<IUserService>();
			A.CallTo(() => mockUserService.IsValidUserName(A<string>.Ignored)).Returns(false);
			var userTestBrowser = MakeTestBrowser<UserModule>(mockUserService: mockUserService);
			
			// When
			var result = userTestBrowser.Post("/user/create", with =>
			{
				with.HttpRequest();
				with.FormValue("username", "<username>");
				with.FormValue("email", "valid@email.com");
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