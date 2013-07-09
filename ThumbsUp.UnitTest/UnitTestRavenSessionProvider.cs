using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThumbsUp.Service.Raven;

namespace ThumbsUp.UnitTest
{
	public class UnitTestRavenSessionProvider : IRavenSessionProvider
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

		private static IDocumentStore DocumentStore
		{
			get
			{
				if (documentStore != null) return documentStore;
				lock (typeof(UnitTestRavenSessionProvider))
				{
					if (documentStore != null) return documentStore;
					documentStore = new EmbeddableDocumentStore { RunInMemory=true };
					documentStore.Initialize();
					IndexCreation.CreateIndexes(typeof(RavenIndexes).Assembly, documentStore);
				}
				return documentStore;
			}
		}

	}
}
