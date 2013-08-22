#region Using
using Shouldly;
using SimpleCrypto;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class PasswordService_IsPasswordValid
	{
		[Fact]
		public void Should_return_true_when_supplied_password_matches_user_password()
		{
			// Given
			var password = MakeFake.Password;
			var fakeUser = new User() { Salt = MakeFake.Salt, Hash = MakeFake.Hash };
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, password);

			// Then
			isValid.ShouldBe(true);
		}

		public void Should_return_false_when_supplied_user_is_missing()
		{
			// Given
			var password = MakeFake.Password;
			User fakeUser = null;
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var isValid = passwordService.IsPasswordValid(fakeUser, password);

			// Then
			isValid.ShouldBe(false);
		}

		[
			Theory,
			InlineData(MakeFake.Password, "", MakeFake.Salt),
			InlineData(MakeFake.Password, MakeFake.Hash, ""),
			InlineData(MakeFake.Password, "", ""),
			InlineData("", MakeFake.Hash, ""),
			InlineData("", MakeFake.Hash, MakeFake.Salt),
			InlineData("", "", MakeFake.Salt),
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
