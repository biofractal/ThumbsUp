using Nancy.Helper;
using Raven.Client;
using SimpleCrypto;
using System;
using System.Linq;
using System.Configuration;
using ThumbsUp.Domain;
using Raven.Helper;

namespace ThumbsUp.Service
{
	public interface IApplicationService
	{
		Application Register(string name);
		Application Transfer(string name, string id);
		Application Get(string id);
		bool IsRegistered(string id);
		bool AuthoriseSingleUseToken(string id);
	}

	public class ApplicationService : IApplicationService
	{
		private readonly IDocumentSession Db;
		public ApplicationService(IRavenSessionProvider documentSessionProvider)
		{
			Db = documentSessionProvider.Get();
		}

		public Application Register(string name)
		{
			var id = Guid.NewGuid().ToString();
			return Register(name, id);
		}

		public Application Register(string name, string id)
		{
			if (string.IsNullOrWhiteSpace(name)) return null;
			var application = new Application()
			{
				Id = id,
				Name = name
			};
			Db.Store(application);
			return application;
		}

		public Application Transfer(string name, string id)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id) || !id.IsGuid()) return null;
			var application = new Application()
			{
				Id = id,
				Name = name
			};
			Db.Store(application);
			return application;
		}

		public Application Get(string id)
		{
			return Db.Load<Application>(id);
		}

		public bool IsRegistered(string id)
		{
			if(string.IsNullOrWhiteSpace(id)) return false;
			return Get(id)!=null;
		}

		public bool AuthoriseSingleUseToken(string id)
		{
			if (string.IsNullOrWhiteSpace(id) || !IsRegistered(id)) return false;
			Db.Advanced.DocumentStore.DatabaseCommands.Delete(id, null);
			Db.ClearStaleIndexes();
			return true;
		}

	}
}
