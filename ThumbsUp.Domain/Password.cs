

namespace ThumbsUp.Domain
{
	public interface IPassword
	{		
		string Clear { get; set; }
		string Hash { get; set; }
		string Salt { get; set; }
	}

	public class Password:IPassword
	{
		public string Clear { get; set; }
		public string Hash { get; set; }
		public string Salt { get; set; }
	}
}
