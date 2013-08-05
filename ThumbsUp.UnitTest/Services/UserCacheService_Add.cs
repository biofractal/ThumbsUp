#region Using
using Nancy.Helper;
using Shouldly;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserCacheService_Add
	{
		[Fact]
		public void Should_return_valid_key_when_user_is_added()
		{
			// Given
			var user = new User() { Id = MakeFake.Guid };
			var userCacheService = new UserCacheService();
			
			// When
			var key = userCacheService.Add(user);
			
			// Then
			key.ShouldNotBe(null);
			key.IsGuid().ShouldBe(true);
		}

		[Fact]
		public void Should_return_null_key_when_user_is_null()
		{
			// Given
			User user = null;
			var userCacheService = new UserCacheService();

			// When
			var key = userCacheService.Add(user);

			// Then
			key.ShouldBe(null);
		}

	}
}
