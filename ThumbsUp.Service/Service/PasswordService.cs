using SimpleCrypto;
using System;
using System.Configuration;

namespace ThumbsUp.Service.Domain
{
	public class PasswordService
	{

		private static readonly int PasswordCharactersCount = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]);
		private static readonly int ForgotPasswordTimeLimitMinutes = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.ForgotPassword.TimeLimit.Minutes"]);

		private readonly ICryptoService Crypto;	

		public PasswordService(ICryptoService cryptoService)
		{
			Crypto = cryptoService;
		}

		public Password Generate()
		{
			return new Password(Crypto, PasswordCharactersCount);
		}

		public bool IsPasswordValid(User user, string clear)
		{
			if (user == null) return false;
			return Crypto.Compute(clear, user.Salt) == user.Hash;
		}
	
		public bool IsForgotPasswordTokenValid(User user, string token)
		{
			return 
				user != null &&
				user.ForgotPasswordRequestToken == token && 
				(DateTime.Now - user.ForgotPasswordRequestDate).Minutes < ForgotPasswordTimeLimitMinutes;
		}	

	}
}
