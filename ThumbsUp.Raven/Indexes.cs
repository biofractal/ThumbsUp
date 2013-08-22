
namespace ThumbsUp.Domain
{
	using Raven.Client.Indexes;
	using System.Linq;

	public class Indexes
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
