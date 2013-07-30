#region Using
using FakeItEasy;
using Shouldly;
using SimpleCrypto;
using System;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserService_CreateUser : _BaseServiceTest
	{
		[Fact]
		public void Should_return_password_when_valid_username_and_password_are_supplied()
		{
			// Given


			var fakePassword = A.Fake<IPassword>();
			A.CallTo(() => fakePassword.Clear).Returns(ValidPassword);
			var fakePasswordService = A.Fake<IPasswordService>();
			A.CallTo(() => fakePasswordService.Generate()).Returns(fakePassword);
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IUserCacheService>(), fakePasswordService);
			var username = ValidUsername;
			var email = ValidEmail;

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
			InlineData(ValidUsername, InvalidEmail)
		]
		public void Should_return_null_when_parameters_are_missing_or_invalid(string username, string email)
		{
			// Given
			var userService = new UserService(A.Dummy<IRavenSessionProvider>(), A.Dummy<IUserCacheService>(), A.Dummy<IPasswordService>());

			// When
			var password = userService.CreateUser(username, email);

			// Then
			password.ShouldBe(null);
		}
	}
}
