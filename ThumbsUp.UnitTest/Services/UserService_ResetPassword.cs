#region Using
using FakeItEasy;
using Raven.Client;
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Service;
using ThumbsUp.Domain;

using Xunit;
using Xunit.Extensions;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Linq;
using System.Collections;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserService_ResetPassword
	{
		[Fact]
		public void Should_return_password_when_valid_user_and_password_are_supplied()
		{
			// Given	
			var user = new User();
			var fakePasswordService = MakeFake.PasswordService();
			A.CallTo(() => fakePasswordService.IsPasswordValid(A<User>.Ignored, A<string>.Ignored)).Returns(true);
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), fakePasswordService);

			// When
			var password = userService.ResetPassword(user);

			// Then
			password.ShouldNotBe(null);
			password.Length.ShouldBe(PasswordService.PasswordCharactersCount);
		}

		public void Should_return_null_when_user_is_null()
		{
			// Given
			User user = null;
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			var password = userService.ResetPassword(user);

			// Then
			password.ShouldBe(null);
		}

	}
}
