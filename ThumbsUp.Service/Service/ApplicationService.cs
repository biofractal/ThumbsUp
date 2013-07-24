using Nancy.Helper;
using Raven.Client;
using SimpleCrypto;
using System;
using System.Configuration;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;

namespace ThumbsUp.Service
{
	public interface IApplicationService
	{
		Application RegisterNew(string name);
		Application RegisterExisting(string name, string id);
		Application Get(string id);
		bool ApplicationIsRegistered(string id);
	}

	public class ApplicationService : IApplicationService
	{
		private readonly string AdminId = "ac8dfe73-4cc2-409c-99bc-36e738f6e29c";
		private readonly IDocumentSession Db;
		public ApplicationService(IRavenSessionProvider documentSessionProvider)
		{
			Db = documentSessionProvider.Get();
		}

		public Application RegisterNew(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return null;
			var application = new Application()
			{
				Id = Guid.NewGuid().ToString(),
				Name = name
			};
			Db.Store(application);
			return application;
		}

		public Application RegisterExisting(string name, string id)
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

		public bool ApplicationIsRegistered(string id)
		{
			if(string.IsNullOrWhiteSpace(id)) return false;
			if (id == AdminId) return true;
			return Get(id)!=null;
		}
	}
}
