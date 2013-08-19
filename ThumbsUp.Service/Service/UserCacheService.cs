using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using ThumbsUp.Domain;
using Nancy.Helper;

namespace ThumbsUp.Service
{
	public interface IUserCacheService
	{
		User GetUser(string key);
		bool Validate(string key);
		string Add(User user);
		bool Remove(string key);
	}

	public class UserCacheService : IUserCacheService
	{
		private readonly MemoryCache Cache = MemoryCache.Default;
		private static readonly int SlidingExpirationMinutes = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.SlidingExpiration.Minutes"]);

		public User GetUser(string key)
		{
			if (string.IsNullOrWhiteSpace(key) || !key.IsGuid()) return null;
			if (!Cache.Contains(key)) return null;
			return (User)Cache[key];
		}

		public bool Validate(string key)
		{
			if (string.IsNullOrWhiteSpace(key) || !key.IsGuid()) return false;
			return Cache.Contains(key);
		}

		public string Add(User user)
		{
			if (user == null) return null;
			var key = Guid.NewGuid().ToString();
			Cache.Add(key, user, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, SlidingExpirationMinutes, 0) });
			return key;
		}

		public bool Remove(string key)
		{
			if (string.IsNullOrWhiteSpace(key) || !key.IsGuid()) return false;
			if (!Cache.Contains(key)) return false;
			Cache.Remove(key);
			return true;
		}
	}
}
