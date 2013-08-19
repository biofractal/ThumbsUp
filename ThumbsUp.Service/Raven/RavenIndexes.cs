using Raven.Client.Indexes;
using System.Linq;
using ThumbsUp.Service.Domain;

namespace ThumbsUp.Service.Raven
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
		public class Application_ByName : AbstractIndexCreationTask<Application>
		{
			public Application_ByName()
			{
				Map = applications => applications.Select(application => new
				{
					application.Name
				});
			}
		}
	}
}
