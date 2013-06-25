using SimpleCrypto;

namespace ThumbsUp.Domain
{
	public class Password
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
