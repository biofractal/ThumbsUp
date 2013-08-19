using SimpleCrypto;

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
		public Password(ICryptoService crypto, int characterCount)
		{
			Clear = RandomPassword.Generate(characterCount);
			Salt = crypto.GenerateSalt();
			Hash = crypto.Compute(Clear, Salt);
		}
		public string Clear { get; set; }
		public string Hash { get; set; }
		public string Salt { get; set; }
	}
}
