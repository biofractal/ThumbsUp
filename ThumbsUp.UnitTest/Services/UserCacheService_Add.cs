#region Using
using Nancy.Helper;
using Shouldly;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class UserCacheService_Add : _BaseServiceTest
	{
		[Fact]
		public void Should_return_valid_key_when_user_is_added()
		{
			// Given
			var userCacheService = new UserCacheService();
			var userId = ValidGuid;
			
			// When
			var key = userCacheService.Add(new User() { Id = userId });
			
			// Then

			key.ShouldNotBe(null);
			key.IsGuid().ShouldBe(true);
		}

		[Fact]
		public void Should_return_null_key_when_user_is_null()
		{
			// Given
			var userCacheService = new UserCacheService();
			User user = null;

			// When
			var key = userCacheService.Add(user);

			// Then
			key.ShouldBe(null);
		}

	}
}
