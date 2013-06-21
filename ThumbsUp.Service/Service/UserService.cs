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

		public string CreateUser(string username, string email)
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

		public User GetUserFromIdentifier(string key)
		{
			if (!Cache.Contains(key)) return null;
			return (User)Cache[key];
		}

		public string ValidateUser(string username, string password)
		{
			var user = GetUserByName(username);
			if (user == null || Crypto.Compute(password, user.Salt) != user.PasswordHash) return null;
			return  AddUserToCache(user);
		}

		public bool ValidateIdentifier(string key)
		{
			return Cache.Contains(key);
		}

		public bool ValidateUserName(string username)
		{
			var user = GetUserByName(username);
			return user == null;
		}

		public string AddUserToCache(User user)
		{
			var key = Guid.NewGuid().ToString();
			Cache.Add(key, user, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, SlidingExpirationMinutes, 0) });
			return key;
		}

		public bool RemoveUserFromCache(string key)
		{
			if (!Cache.Contains(key)) return false;
			Cache.Remove(key);
			return true;
		}

		private User GetUserByName(string username)
		{
			return Db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName==username);
		}
	}
}
