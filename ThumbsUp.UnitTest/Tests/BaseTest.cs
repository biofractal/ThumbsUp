#region Using

using Nancy.Testing;
using System.Configuration;


#endregion

namespace ThumbsUp.UnitTest.Tests
{
	public class BaseTest
	{
		public readonly string ApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];

		public Browser NewBrowser()
		{
			var bootstrapper = new UnitTestBootstrapper();
			return new Browser(bootstrapper);
		}
	}
}