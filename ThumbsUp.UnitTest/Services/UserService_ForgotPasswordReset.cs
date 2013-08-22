#region Using
using FakeItEasy;
using Raven.Helper;
using Shouldly;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserService_ForgotPasswordReset
	{
		[Fact]
		public void Should_return_valid_password_when_valid_user_and_email_are_supplied()
		{
			// Given

			var user = new User();
			var fakePasswordService = MakeFake.PasswordService();
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), fakePasswordService);

			// When
			var password = userService.ForgotPasswordReset(user);

			// Then
			password.ShouldNotBe(null);
			password.Length.ShouldBe(PasswordService.PasswordCharactersCount);
		}

		[Fact]
		public void Should_set_user_properties_when_valid_user_and_email_are_supplied()
		{
			// Given

			var user = new User() { ForgotPasswordRequestToken = MakeFake.Guid };
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			userService.ForgotPasswordReset(user);

			// Then
			user.Salt.ShouldNotBe(null);
			user.Hash.ShouldNotBe(null);
			user.ForgotPasswordRequestToken.ShouldBe(string.Empty);
		}

		public void Should_return_null_token_when_user_is_null()
		{
			// Given
			User user = null;
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			var password = userService.ForgotPasswordReset(user);

			// Then
			password.ShouldBe(null);
		}
	}
}
