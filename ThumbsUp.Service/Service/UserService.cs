using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using ThumbsUp.Service.Domain;
using ThumbsUp.Service.Raven;
using Nancy.Helper;

namespace ThumbsUp.Service
{
	public interface IUserService
	{
		string CreateUser(string username, string email);
		string ResetPassword(string username, string candidatePassword);
		string ForgotPasswordRequest(string username, string email);
		string ForgotPasswordReset(string username, string token);
		string ValidateUser(string username, string candidatePassword);
		bool IsValidUserName(string username);
		User GetUserFromIdentifier(string thumbKey);
		bool ValidateIdentifier(string thumbKey);
		string AddUserToCache(User user);
		bool RemoveUserFromCache(string thumbKey);
	}

	public class UserService : IUserService
	{
		private readonly IUserCacheService Cache;
		private readonly IDocumentSession Db;
		private readonly IPasswordService Pwd;

		public UserService(IRavenSessionProvider documentSessionProvider, IUserCacheService cache, IPasswordService passwordService)
		{
			Db = documentSessionProvider.Get();
			Pwd = passwordService;
			Cache = cache;
		}

		public string CreateUser(string username, string email)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email)) return null;
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
			if (string.IsNullOrWhiteSpace(username)) return null;
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
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email)) return null;
			var user = GetUserByName(username);
			if (user == null || user.Email != email) return null;
			var token = Guid.NewGuid().ToString();
			user.ForgotPasswordRequestToken = token;
			user.ForgotPasswordRequestDate = DateTime.Now;
			Db.Store(user);
			return token;
		}

		public string ForgotPasswordReset(string username, string token)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token)) return null;
			var user = GetUserByName(username);
			if (user == null || !Pwd.IsForgotPasswordTokenValid(user, token)) return null;
			var password = Pwd.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			user.ForgotPasswordRequestToken = string.Empty;
			Db.Store(user);
			return password.Clear;
		}

		public string ValidateUser(string username, string candidatePassword)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(candidatePassword)) return null;
			var user = GetUserByName(username);
			if (user == null) return null;
			if (!Pwd.IsPasswordValid(user, candidatePassword)) return null;
			return AddUserToCache(user);
		}

		public bool IsValidUserName(string username)
		{
			if (string.IsNullOrWhiteSpace(username)) return false;
			var user = GetUserByName(username);
			return user == null;
		}

		public User GetUserFromIdentifier(string thumbKey)
		{
			return Cache.GetUser(thumbKey);
		}

		public bool ValidateIdentifier(string thumbKey)
		{
			return Cache.Validate(thumbKey);
		}

		public string AddUserToCache(User user)
		{
			return Cache.Add(user);
		}

		public bool RemoveUserFromCache(string thumbKey)
		{
			return Cache.Remove(thumbKey);
		}

		private User GetUserByName(string username)
		{
			return Db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName == username);
		}
	}
}
