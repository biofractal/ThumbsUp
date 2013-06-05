using Raven.Client;
using SimpleCrypto;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using ThumbsUp.Domain;
using ThumbsUp.Raven;

namespace ThumbsUp.Service
{
	public class UserService
	{
		private readonly ICryptoService Crypto;
		private readonly IDocumentSession Db;
		private static readonly int SlidingExpirationMinutes = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.SlidingExpiration.Minutes"]);

		public UserService(ICryptoService cryptoService, RavenSessionProvider documentSessionProvider)
		{
			Crypto = cryptoService;
			Db = documentSessionProvider.Get();
		}

		public string Create(string username, string email)
		{
			var password = RandomPassword.Generate(12);
			var user = new User()
			{
				Id = Guid.NewGuid().ToString(),
				Salt = Crypto.GenerateSalt(),
				PasswordHash = Crypto.Compute(password),
				Email = email,
				UserName = username
			};
			Db.Store(user);
			return password;
		}

		public Tuple<Guid, User> GetUserAndKey(string username, string password)
		{
			var user = Db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName == username);
			if (user == null) return null;
			if (Crypto.Compute(password, user.Salt) != user.PasswordHash) return null;
			var thumbKey = Guid.NewGuid();
			MemoryCache.Default.Add(thumbKey.ToString(), user, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, SlidingExpirationMinutes, 0) });
			return new Tuple<Guid, User>(thumbKey, user);
		}

		public bool Validate(string identifier)
		{
			return MemoryCache.Default.Contains(identifier);
		}

		public void Remove(string identifier)
		{
			if (MemoryCache.Default.Contains(identifier)) MemoryCache.Default.Remove(identifier);
		}
	}
}
