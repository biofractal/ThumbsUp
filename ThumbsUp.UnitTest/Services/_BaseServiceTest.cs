#region Using
using FakeItEasy;
using Raven.Client;
using ThumbsUp.Service.Raven;

#endregion


namespace ThumbsUp.UnitTest.Services
{
	public class _BaseServiceTest
	{
		public readonly IRavenSessionProvider DummyRavenSessionProvider = A.Dummy<IRavenSessionProvider>();
		public const string ValidGuid = "0e2ae555-9fc7-4b89-8ea4-a8b63097c50a";
		public const string InvalidGuid = "<invalid-guid>";


		public const string ValidHash = "W5OS/v3YxIEYRvMtYc7TYDFbbvucPaqwuThGe2wbXlVkmfkHh7InpXfcw0V8ansVDdqA2zhAooKkVKIjv3x14g==";
		public const string ValidSalt = "100000.4n9+TkbFFA7Do1QBbHgkWb4dM5WL5iLJLgpwEarnJnNoNw==";
		public const string ValidPassword = "2Qr}b8_N$yZ6";

		public IRavenSessionProvider MakeFakeRavenSessionProvider<T>(T instanceToLoad)
		{
			var fakeDocumentSession = A.Fake<IDocumentSession>();
			A.CallTo(() => fakeDocumentSession.Load<T>(A<string>.Ignored)).Returns(instanceToLoad);
			var fakeRavenSessionProvider = A.Fake<IRavenSessionProvider>();
			A.CallTo(() => fakeRavenSessionProvider.Get()).Returns(fakeDocumentSession);
			return fakeRavenSessionProvider;
		}
	}
}
