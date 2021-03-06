﻿

namespace ThumbsUp.Client
{
	using Raven.Client;
	using Raven.Client.Document;
	using Raven.Helper;
	using System;
	using System.Linq;
	using System.Threading;
	using ThumbsUp.Domain;

	public static class ThumbsUpAdmin
	{
		public static readonly IRavenSessionProvider RavenSessionProvider = new RavenSessionProvider<Indexes>();

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
				var application = db.Query<Application, Indexes.Application_ByName>().FirstOrDefault(app => app.Name == name);
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
