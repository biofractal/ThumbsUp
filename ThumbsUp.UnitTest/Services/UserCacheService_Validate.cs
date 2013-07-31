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
	public class UserCacheService_Validate : _BaseTest
	{
		[Fact]
		public void Should_return_true_when_key_is_known()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var key = userCacheService.Add(new User() { Id = userId });

			// When
			var isValid = userCacheService.Validate(key);

			// Then
			isValid.ShouldBe(true);
		}

		[Fact]
		public void Should_return_false_when_key_is_unknown()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			var unknownKey = Guid.NewGuid().ToString();
			userCacheService.Add(new User() { Id = userId });

			// When
			var isValid = userCacheService.Validate(unknownKey);

			// Then
			isValid.ShouldBe(false);
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
			var isValid = userCacheService.Validate(key);

			// Then
			isValid.ShouldBe(false);
		}
	}
}
