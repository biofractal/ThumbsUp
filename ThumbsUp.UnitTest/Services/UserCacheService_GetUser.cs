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
	public class UserCacheService_GetUser : _BaseServiceTest
	{
		[Fact]
		public void Should_return_user_when_key_is_known()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var key = userCacheService.Add(new User() { Id = userId });

			// When
			var user = userCacheService.GetUser(key);

			// Then
			user.ShouldNotBe(null);
			user.Id.ShouldBe(userId);
		}

		[Fact]
		public void Should_return_null_when_key_is_unknown()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var unknownKey = Guid.NewGuid().ToString();
			userCacheService.Add(new User() { Id = userId });

			// When
			var user = userCacheService.GetUser(unknownKey);

			// Then
			user.ShouldBe(null);
		}

		[
			Theory(),
			InlineData(""),
			InlineData(InvalidGuid),
		]
		public void Should_return_null_when_parameters_are_missing_or_invalid(string key)
		{
			// Given
			var userCacheService = new UserCacheService();

			// When
			var user = userCacheService.GetUser(key);

			// Then
			user.ShouldBe(null);
		}
	}
}
