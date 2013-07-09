using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;

namespace ThumbsUp.Service
{
	public class UserService
	{
		private readonly MemoryCache Cache = MemoryCache.Default;
		private readonly IDocumentSession Db;
		private readonly PasswordService Pwd;
		private static readonly int SlidingExpirationMinutes = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.SlidingExpiration.Minutes"]);
		private static readonly int PasswordCharactersCount = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]);

		public UserService(IRavenSessionProvider documentSessionProvider, PasswordService passwordService)
		{
			Db = documentSessionProvider.Get();
			Pwd = passwordService;
		}

		public string CreateUser(string username, string email)
		{
			var password = Pwd.Generate();
			var user = new User()
			{
				Id = Guid.NewGuid().ToString(),
				Email = email,
				UserName = username,
				Salt = password.Salt,
				Hash = password.Hash
			};
			Db.Store(user);
			return password.Clear;
		}

		public string ResetPassword(string username, string candidatePassword)
		{
			var user = GetUserByName(username);
			if (user == null) return null;
			if (!Pwd.IsPasswordValid(user, candidatePassword)) return null;
			var password = Pwd.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			Db.Store(user);
			return password.Clear;
		}

		public string ForgotPasswordRequest(string username, string email)
		{
			var user = GetUserByName(username);
			if (user == null || user.Email!=email) return null;
			var token = Guid.NewGuid().ToString();
			user.ForgotPasswordRequestToken = token;
			user.ForgotPasswordRequestDate = DateTime.Now;
			Db.Store(user);
			return token;
		}

		public string ForgotPasswordReset(string username, string token)
		{
			var user = GetUserByName(username);
			if (user == null || !Pwd.IsForgotPasswordTokenValid(user, token)) return null;
			var password = Pwd.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			user.ForgotPasswordRequestToken = string.Empty;
			Db.Store(user);
			return password.Clear;
		}

		public User GetUserFromIdentifier(string key)
		{
			if (!Cache.Contains(key)) return null;
			return (User)Cache[key];
		}

		public string ValidateUser(string username, string candidatePassword)
		{
			var user = GetUserByName(username);
			if (user == null) return null;
			if (!Pwd.IsPasswordValid(user, candidatePassword)) return null;
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
