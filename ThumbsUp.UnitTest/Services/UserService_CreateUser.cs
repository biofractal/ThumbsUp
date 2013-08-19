#region Using
using FakeItEasy;
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Service;
using ThumbsUp.Domain;

using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserService_CreateUser
	{
		[Fact]
		public void Should_return_password_when_valid_username_and_password_are_supplied()
		{
			// Given
			var username = MakeFake.Username;
			var email = MakeFake.Email;
			var fakePasswordService = MakeFake.PasswordService();
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), fakePasswordService);

			// When
			var password = userService.CreateUser(username, email);

			// Then
			password.ShouldNotBe(null);
			password.Length.ShouldBe(PasswordService.PasswordCharactersCount);
		}


		[
			Theory(),
			InlineData("", ""),
			InlineData(null, null),
			InlineData(MakeFake.Username, MakeFake.InvalidEmail)
		]
		public void Should_return_null_when_parameters_are_missing_or_invalid(string username, string email)
		{
			// Given
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IPasswordService>());

			// When
			var password = userService.CreateUser(username, email);

			// Then
			password.ShouldBe(null);
		}
	}
}
