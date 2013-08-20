using Nancy.Helper;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Linq;
using System.Threading;
using ThumbsUp.Domain;

namespace ThumbsUp.Client
{
	public static class ThumbsUpAdmin
	{
		public static readonly IRavenSessionProvider RavenSessionProvider = new RavenSessionProvider<RavenIndexes>();

		public static string GenerateSingleUseToken()
		{
			using (var db = RavenSessionProvider.Get())
			{
				var name = Environment.MachineName + "-single-use-token";
				var application = new Application() { Id = Guid.NewGuid().ToString(), Name = name };
				db.Store(application);
				db.SaveChanges();
				db.ClearStaleIndexes();
				return application.Id;
			}
		}

		public static string GetConsoleApplicationId(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return null;
			using (var db = RavenSessionProvider.Get())
			{
				var application = db.Query<Application, RavenIndexes.Application_ByName>().FirstOrDefault(app => app.Name == name);
				if (application == null)
				{
					application = new Application() { Id = Guid.NewGuid().ToString(), Name = name };
					db.Store(application);
					db.SaveChanges();
					db.ClearStaleIndexes();
				}
				return application.Id;
			}
		}

		public static void ClearStaleIndexes(this IDocumentSession db)
		{
			while (((DocumentSession)db).DatabaseCommands.GetStatistics().StaleIndexes.Length != 0) Thread.Sleep(10);
		}

	}
}
