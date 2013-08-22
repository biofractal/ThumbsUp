using System;


namespace ThumbsUp.Client
{

	public interface IThumbsUpUser
	{
		string Id { get; set; }
		Guid ThumbKey { get; set; }
		string UserName { get; set; }
		string Email { get; set; }
	}

	public class ThumbsUpUser : IThumbsUpUser
	{
		public string Id { get; set; }
		public Guid ThumbKey { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
	}
}
