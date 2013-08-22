#region Using
using FakeItEasy;
using Nancy.Helper;
using Raven.Helper;
using Shouldly;
using System;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
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
