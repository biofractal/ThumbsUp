using FakeItEasy;
using Raven.Client;
using ThumbsUp.Service;
using ThumbsUp.Domain;
using Nancy.Helper;


namespace ThumbsUp.UnitTest
{
	static class MakeFake
	{

		public const string Guid = "0e2ae555-9fc7-4b89-8ea4-a8b63097c50a";
		public const string Hash = "W5OS/v3YxIEYRvMtYc7TYDFbbvucPaqwuThGe2wbXlVkmfkHh7InpXfcw0V8ansVDdqA2zhAooKkVKIjv3x14g==";
		public const string Salt = "100000.4n9+TkbFFA7Do1QBbHgkWb4dM5WL5iLJLgpwEarnJnNoNw==";
		public const string Password = "2Qr}b8_N$yZ6";
		public const string Username = "<valid-username>";
		public const string Email = "valid@email.com";
		public const string Name = "<valid-name>";

		public const string InvalidEmail = "<invalid-email>";
		public const string InvalidGuid = "<invalid-guid>";

		public static IRavenSessionProvider RavenSessionProvider<T>(T instanceToLoad)
		{
			var fakeDocumentSession = A.Fake<IDocumentSession>();
			A.CallTo(() => fakeDocumentSession.Load<T>(A<string>.Ignored)).Returns(instanceToLoad);
			var fakeRavenSessionProvider = A.Fake<IRavenSessionProvider>();
			A.CallTo(() => fakeRavenSessionProvider.Get()).Returns(fakeDocumentSession);
			return fakeRavenSessionProvider;
		}

		public static IUserService UserService(User userToReturn = null)
		{
			var fakeUserService = A.Fake<IUserService>();
			A.CallTo(() => fakeUserService.GetUserByName(A<string>.Ignored)).Returns(userToReturn);
			return fakeUserService;
		}

		public static IUserCacheService UserCacheService(User userToReturn = null)
		{
			var fakeUserCacheService = A.Fake<IUserCacheService>();
			A.CallTo(() => fakeUserCacheService.GetUser(A<string>.Ignored)).Returns(userToReturn);
			return fakeUserCacheService;
		}

		public static IPasswordService PasswordService()
		{
			var fakePassword = A.Fake<IPassword>();
			A.CallTo(() => fakePassword.Clear).Returns(Password);
			var fakePasswordService = A.Fake<IPasswordService>();
			A.CallTo(() => fakePasswordService.Generate()).Returns(fakePassword);
			return fakePasswordService;
		}

		public static IApplicationService ApplicationService()
		{
			return A.Fake<IApplicationService>();
		}

	}
}
