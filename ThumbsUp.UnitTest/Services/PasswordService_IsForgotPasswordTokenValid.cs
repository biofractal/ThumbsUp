#region Using
using FakeItEasy;
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class PasswordService_IsForgotPasswordTokenValid
	{
		[Fact]
		public void Should_return_true_when_password_matches_user_password()
		{
			// Given
			var token = MakeFake.Guid;
			var fakeUser = new User() { ForgotPasswordRequestToken= token, ForgotPasswordRequestDate = DateTime.Now };
			var passwordService = new PasswordService(A.Dummy<ICryptoService>());

			// When
			var isValid = passwordService.IsForgotPasswordTokenValid(fakeUser, token);

			// Then
			isValid.ShouldBe(true);
		}

		[Fact]
		public void Should_return_false_when_minutes_elapsed_since_the_token_was_requested_is_greater_than_the_ForgotPasswordTimeLimit()
		{
			// Given
			var token = MakeFake.Guid;
			var requestDate = DateTime.MinValue;
			var fakeUser = new User() { ForgotPasswordRequestToken = token, ForgotPasswordRequestDate = requestDate };
			var passwordService = new PasswordService(A.Dummy<ICryptoService>());

			// When
			var isValid = passwordService.IsForgotPasswordTokenValid(fakeUser, token);

			// Then
			isValid.ShouldBe(false);
		}

		public void Should_return_false_when_user_is_missing()
		{
			// Given
			var token = MakeFake.Guid;
			User fakeUser = null;
			var passwordService = new PasswordService(A.Dummy<ICryptoService>());

			// When
			var isValid = passwordService.IsForgotPasswordTokenValid(fakeUser, token);

			// Then
			isValid.ShouldBe(false);
		}

		[
			Theory(),
			InlineData(""),
			InlineData(MakeFake.InvalidGuid),
		]
		public void Should_return_false_when_token_is_missing_or_invalid(string token)
		{
			// Given
			var fakeUser = new User();
			var passwordService = new PasswordService(A.Dummy<ICryptoService>());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, token);

			// Then
			isValid.ShouldBe(false);
		}


	}
}