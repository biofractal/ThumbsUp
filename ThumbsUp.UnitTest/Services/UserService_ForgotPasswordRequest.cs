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
	public class UserService_ForgotPasswordRequest
	{
		[Fact]
		public void Should_return_token_when_valid_user_and_email_are_supplied()
		{
			// Given

			var user = new User();
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			var token = userService.ForgotPasswordRequest(user);

			// Then
			token.ShouldNotBe(null);
			token.ToString().IsGuid().ShouldBe(true);
			user.ForgotPasswordRequestToken.ShouldBe(token);
			user.ForgotPasswordRequestDate.Ticks.ShouldBeLessThan(DateTime.Now.AddSeconds(1).Ticks);
		}

		public void Should_return_null_token_when_user_is_null()
		{
			// Given
			User user = null;
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			var token = userService.ForgotPasswordRequest(user);

			// Then
			token.ShouldBe(null);
		}
	}
}
