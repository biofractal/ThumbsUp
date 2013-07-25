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
	public class UserCacheService_Remove : _BaseServiceTest
	{
		[Fact]
		public void Should_return_true_when_key_is_known()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var key = userCacheService.Add(new User() { Id = userId });

			// When
			var success = userCacheService.Remove(key);
			var user = userCacheService.GetUser(key);

			// Then
			success.ShouldBe(true);
			user.ShouldBe(null);
		}

		[Fact]
		public void Should_return_false_when_key_is_unknown()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var unknownKey = Guid.NewGuid().ToString();
			var key = userCacheService.Add(new User() { Id = userId });

			// When
			var success = userCacheService.Remove(unknownKey);
			var user = userCacheService.GetUser(key);
			// Then
			success.ShouldBe(false);
			user.ShouldNotBe(null);
		}

		[
			Theory(),
			InlineData(""),
			InlineData(InvalidGuid),
		]
		public void Should_return_false_when_parameters_are_missing_or_invalid(string key)
		{
			// Given
			var userCacheService = new UserCacheService();

			// When
			var success = userCacheService.Remove(key);

			// Then
			success.ShouldBe(false);
		}
	}
}
