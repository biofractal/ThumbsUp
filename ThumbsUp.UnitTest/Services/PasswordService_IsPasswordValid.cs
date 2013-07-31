#region Using
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class PasswordService_IsPasswordValid : _BaseTest
	{
		[Fact]
		public void Should_return_true_when_supplied_password_matches_user_password()
		{
			// Given
			var password = ValidPassword;
			var fakeUser = new User() { Salt = ValidSalt, Hash = ValidHash };
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, password);

			// Then
			isValid.ShouldBe(true);
		}

		public void Should_return_false_when_supplied_user_is_missing()
		{
			// Given
			var password = ValidPassword;
			User fakeUser = null;
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, password);

			// Then
			isValid.ShouldBe(false);
		}

		[
			Theory,
			InlineData(ValidPassword, "", ValidSalt),
			InlineData(ValidPassword, ValidHash, ""),
			InlineData(ValidPassword, "", ""),
			InlineData("", ValidHash, ""),
			InlineData("", ValidHash, ValidSalt),
			InlineData("", "", ValidSalt),
			InlineData("", "", "")
		]
		public void Should_return_false_when_supplied_parameters_are_missing_or_invalid(string password, string hash, string salt)
		{
			// Given
			var fakeUser = new User() { Salt = salt, Hash = hash };
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, password);

			// Then
			isValid.ShouldBe(false);
		}


	}
}
