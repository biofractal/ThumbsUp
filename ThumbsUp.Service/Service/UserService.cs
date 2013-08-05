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
		string ForgotPasswordRequest(User user);
		string ForgotPasswordReset(User user);
		User GetUserByName(string username);
	}

	public class UserService : IUserService
	{
		private readonly IDocumentSession db;
		private readonly IPasswordService passwordService;

		public UserService(IRavenSessionProvider documentSessionProvider, IPasswordService pwd)
		{
			db = documentSessionProvider.Get();
			passwordService = pwd;
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

		public string ForgotPasswordRequest(User user)
		{
			if (user==null) return null;
			var token = Guid.NewGuid().ToString();
			user.ForgotPasswordRequestToken = token;
			user.ForgotPasswordRequestDate = DateTime.Now;
			db.Store(user);
			return token;
		}

		public string ForgotPasswordReset(User user)
		{
			if (user == null) return null;
			var password = passwordService.Generate();
			user.Salt = password.Salt;
			user.Hash = password.Hash;
			user.ForgotPasswordRequestToken = string.Empty;
			db.Store(user);
			return password.Clear;
		}

		public User GetUserByName(string username)
		{
			return db.Query<User, RavenIndexes.User_ByCredentials>().FirstOrDefault(x => x.UserName == username);
		}
	}
}
