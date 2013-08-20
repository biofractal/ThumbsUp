using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System;
using System.Configuration;

namespace ThumbsUp.RavenDB
{
	public interface IRavenSessionProvider
	{ 
		bool SessionInitialized { get; set; }
		void SaveChangesAfterRequest();
		IDocumentSession Get();
	}

	public class RavenSessionProvider:IRavenSessionProvider
	{
		private static IDocumentStore documentStore;
		private IDocumentSession documentSession;
		public bool SessionInitialized { get; set; }

		public IDocumentSession Get()
		{
			SessionInitialized = true;
			documentSession = documentSession ?? (documentSession = DocumentStore.OpenSession());
			return documentSession;
		}
	
		public void SaveChangesAfterRequest()
		{
			if (!this.SessionInitialized) return;
			documentSession.SaveChanges();
			documentSession.Dispose();
		}

		private static IDocumentStore DocumentStore
		{
			get
			{
				if (documentStore != null) return documentStore;
				lock (typeof(RavenSessionProvider))
				{
					if (documentStore != null) return documentStore;

					documentStore = new DocumentStore
					{
						ConnectionStringName = ConnectionStringName
					};
					documentStore.Initialize();
					IndexCreation.CreateIndexes(typeof(RavenIndexes).Assembly, documentStore);
				}
				return documentStore;
			}
		}

		private static string ConnectionStringName
		{
			get
			{
				var customConnection = ConfigurationManager.ConnectionStrings[Environment.MachineName] != null;
				var connectionStringName = customConnection ? Environment.MachineName : "RavenDB";
				return connectionStringName;
			}
		}

	}
}
