﻿using Raven.Client.Indexes;
using System.Linq;
using ThumbsUp.Domain;

namespace ThumbsUp.Raven
{
	public class RavenIndexes
	{
		public class User_ByCredentials : AbstractIndexCreationTask<User>
		{
			public User_ByCredentials()
			{
				Map = users => users.Select(user => new
				{
					user.Id,
					user.Salt,
					PasswordHash = user.Hash,
					user.Email,
					user.UserName
				});
			}
		}

	}
}