#region Using
using Shouldly;
using ThumbsUp.Domain;
using ThumbsUp.Service;
using Xunit;
using Xunit.Extensions;

#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserCacheService_Remove
	{
		[Fact]
		public void Should_return_success_is_true_when_removing_known_key()
		{
			// Given
			var user = new User() { Id = MakeFake.Guid };
			var userCacheService = new UserCacheService();
			var key = userCacheService.Add(user);
			
			// When
			var success = userCacheService.Remove(key);

			// Then
			success.ShouldBe(true);
		}

		[Fact]
		public void Should_return_null_user_after_user_is_removed()
		{
			// Given
			var userId = MakeFake.Guid;
			var userCacheService = new UserCacheService();
			var key = userCacheService.Add(new User() { Id = userId });
			
			// When
			userCacheService.Remove(key);
			var user = userCacheService.GetUser(key);

			// Then
			user.ShouldBe(null);
		}

		[Fact]
		public void Should_return_success_is_false_when_key_is_unknown()
		{
			// Given
			var user = new User() { Id = MakeFake.Guid };
			var unknownKey = MakeFake.Guid;
			var userCacheService = new UserCacheService();
			var key = userCacheService.Add(user);

			// When
			var success = userCacheService.Remove(unknownKey);

			// Then
			success.ShouldBe(false);
		}

		[
			Theory(),
			InlineData(""),
			InlineData(MakeFake.InvalidGuid),
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
