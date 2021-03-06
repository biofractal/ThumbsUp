﻿#region Using
using Shouldly;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserCacheService_GetUser
	{
		[Fact]
		public void Should_return_user_when_key_is_known()
		{
			// Given
			var userId = MakeFake.Guid;
			var userCacheService = new UserCacheService();
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
			var userId = MakeFake.Guid;
			var userCacheService = new UserCacheService();
			var unknownKey = MakeFake.Guid;
			userCacheService.Add(new User() { Id = userId });

			// When
			var user = userCacheService.GetUser(unknownKey);

			// Then
			user.ShouldBe(null);
		}

		[
			Theory(),
			InlineData(""),
			InlineData(MakeFake.InvalidGuid),
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
