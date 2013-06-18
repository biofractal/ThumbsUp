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
		private readonly MemoryCache Cache = MemoryCache.Default;
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

		public dynamic GetUserFromIdentifier(string key)
		{
			if (!Exists(key)) return null;
			var user = (User)Cache[key];
			return new {User= new {Id = user.Id, UserName = user.UserName, Email = user.Email }};
		}

		public dynamic ValidateUser(string username, string password)
		{
			var user = Db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName == username);
			if (user == null || Crypto.Compute(password, user.Salt) != user.PasswordHash) return null;
			var key= AddUserToCache(user);
			return new { ThumbKey = key };
		}

		public string AddUserToCache(User user)
		{
			var key = Guid.NewGuid().ToString();
			Cache.Add(key, user, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, SlidingExpirationMinutes, 0) });
			return key;
		}

		public bool Exists(string key)
		{
			return Cache.Contains(key);
		}

		public bool Remove(string key)
		{
			if (!Exists(key)) return false;
			Cache.Remove(key);
			return true;
		}

	}
}
