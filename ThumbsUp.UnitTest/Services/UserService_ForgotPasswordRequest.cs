#region Using
using Nancy.Helper;
using FakeItEasy;
using Raven.Client;
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;
using Xunit;
using Xunit.Extensions;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Linq;
using System.Collections;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserService_ForgotPasswordRequest : _BaseTest
	{
		[Fact]
		public void Should_return_token_when_valid_user_and_email_are_supplied()
		{
			// Given
			var email = ValidEmail;
			var user = new User() { Email=email};
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IUserCacheService>(), A.Dummy<IPasswordService>());

			// When
			var token = userService.ForgotPasswordRequest(user, email);

			// Then
			token.ShouldNotBe(null);
			token.ToString().IsGuid().ShouldBe(true);
			user.ForgotPasswordRequestToken.ShouldBe(token);
			user.ForgotPasswordRequestDate.Ticks.ShouldBeLessThan(DateTime.Now.AddSeconds(1).Ticks);
		}

		public void Should_return_null_Token_when_user_is_null()
		{
			// Given
			var email = ValidEmail;
			User user = null;
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IUserCacheService>(), A.Dummy<IPasswordService>());

			// When
			var token = userService.ForgotPasswordRequest(user, email);

			// Then
			token.ShouldBe(null);
		}

		[
			Theory(),
			InlineData(null),
			InlineData(""),
		]
		public void Should_return_null_Token_when_email_is_null_or_missing(string email)
		{
			// Given
			User user = null;
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IUserCacheService>(), A.Dummy<IPasswordService>());

			// When
			var password = userService.ForgotPasswordRequest(user, email);

			// Then
			password.ShouldBe(null);
		}
	}
}
