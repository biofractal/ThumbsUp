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
		string ResetPassword(User user);
		string ForgotPasswordRequest(User user, string email);
		string ForgotPasswordReset(string username, string token);
		string ValidateUser(string username, string candidatePassword);
		bool IsValidUserName(string username);
		User GetUserFromIdentifier(string thumbKey);
		User GetUserByName(string username);
		bool ValidateIdentifier(string thumbKey);
		string AddUserToCache(User user);
		bool RemoveUserFromCache(string thumbKey);
	}

	public class UserService : IUserService
	{
		private readonly IUserCacheService cacheService;
		private readonly IDocumentSession db;
		private readonly IPasswordService passwordService;

		public UserService(IRavenSessionProvider documentSessionProvider, IUserCacheService cache, IPasswordService pwd)
		{
			db = documentSessionProvider.Get();
			passwordService = pwd;
			cacheService = cache;
		}

		public string CreateUser(string username, string email)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || !email.IsEmail()) return null;
			var password = passwordService.Generate();
			var user = new User()
			{
				Id = Guid.NewGuid().ToString(),
				Email = email,
				UserName = username,
				Salt = password.Salt,
				Hash = password.Hash
			};
			db.Store(user);
			return password.Clear;
		}

		public string ResetPassword(User user)
		{
			if (user==null) return null;
			var password = passwordService.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			db.Store(user);
			return password.Clear;
		}

		public string ForgotPasswordRequest(User user, string email)
		{
			if (user==null || string.IsNullOrWhiteSpace(email) || user.Email != email) return null;
			var token = Guid.NewGuid().ToString();
			user.ForgotPasswordRequestToken = token;
			user.ForgotPasswordRequestDate = DateTime.Now;
			db.Store(user);
			return token;
		}

		public string ForgotPasswordReset(string username, string token)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token)) return null;
			var user = GetUserByName(username);
			if (user == null || !passwordService.IsForgotPasswordTokenValid(user, token)) return null;
			var password = passwordService.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			user.ForgotPasswordRequestToken = string.Empty;
			db.Store(user);
			return password.Clear;
		}

		public string ValidateUser(string username, string candidatePassword)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(candidatePassword)) return null;
			var user = GetUserByName(username);
			if (user == null) return null;
			if (!passwordService.IsPasswordValid(user, candidatePassword)) return null;
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
			return cacheService.GetUser(thumbKey);
		}

		public bool ValidateIdentifier(string thumbKey)
		{
			return cacheService.Validate(thumbKey);
		}

		public string AddUserToCache(User user)
		{
			return cacheService.Add(user);
		}

		public bool RemoveUserFromCache(string thumbKey)
		{
			return cacheService.Remove(thumbKey);
		}

		public User GetUserByName(string username)
		{
			return db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName == username);
		}
	}
}
