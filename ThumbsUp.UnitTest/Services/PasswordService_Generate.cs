#region Using
using Shouldly;
using SimpleCrypto;
using ThumbsUp.Service;
using Xunit;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class PasswordService_Generate
	{
		[Fact]
		public void Should_return_generated_password_of_correct_length()
		{
			// Given
			var passwordService = new PasswordService(new PBKDF2());

			// When
			var password = passwordService.Generate();

			// Then
			password.ShouldNotBe(null);
			password.Hash.ShouldNotBe(null);
			password.Salt.ShouldNotBe(null);
			password.Clear.ShouldNotBe(null);
			password.Clear.Length.ShouldBe(PasswordService.PasswordCharactersCount);
		}
	}
}
