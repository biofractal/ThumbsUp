#region Using

using Nancy;
using Nancy.Helper;
using Nancy.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class UserCreate : _BaseTest
	{		
		[Fact]
		public void Should_return_password_when_user_is_created()
		{
			// Given
			var browser = StdBrowser();

			// When
			var result = browser.Post("/user/create", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("username", "biofractal");
				with.FormValue("email", "biofractal@email.com");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.OK);

			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["Password"].ShouldNotBe(null);
			payload["Password"].ToString().Length.ShouldBe(int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]));
		}

		#region Errors

		[Fact]
		public void Should_return_MissingParameters_error_when_user_is_created_with_missing_params()
		{
			TestMissingParams("/user/create");
		}

		[Fact]
		public void Should_return_InvalidParameters_error_when_user_is_created_with_invalid_email()
		{
			var formValues = new Dictionary<string, string>() { { "username", "<test>" }, { "email", "<invalid>" } };
			TestInvalidParams("/user/create", formValues);
		}

		[Fact]
		public void Should_return_UserNameTaken_error_user_is_created_with_taken_username()
		{
			// Given
			var browser = StdBrowser();
			browser.Post("/user/create", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("username", "duplicate");
				with.FormValue("email", "duplicate@email.com");
			});
			
			// When
			var result = browser.Post("/user/create", with =>
			{
				with.HttpRequest();
				with.FormValue("applicationid", ApplicationId);
				with.FormValue("username", "duplicate");
				with.FormValue("email", "duplicate@email.com");
			});

			// Then
			result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			
			var payload = result.Body.DeserializeJson<Dictionary<string, object>>();
			payload["ErrorCode"].ShouldBe(3);
			payload["ErrorMessage"].ShouldBe("The UserName has already been taken");
		}		
		#endregion
	}
}