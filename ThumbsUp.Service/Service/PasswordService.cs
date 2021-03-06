﻿using SimpleCrypto;
using System;
using System.Configuration;
using Nancy.Helper;
using ThumbsUp.Domain;

namespace ThumbsUp.Service
{
	public interface IPasswordService
	{
		IPassword Generate();
		bool IsPasswordValid(User user, string clear);
		bool IsForgotPasswordTokenValid(User user, string token);
	}

	public class PasswordService : IPasswordService
	{
		public static readonly int PasswordCharactersCount = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.PasswordCharacters.Count"]);
		public static readonly int ForgotPasswordTimeLimitMinutes = int.Parse(ConfigurationManager.AppSettings["ThumbsUp.ForgotPassword.TimeLimit.Minutes"]);

		private readonly ICryptoService cryptoService;

		public PasswordService(ICryptoService crypto)
		{
			cryptoService = crypto;
		}

		public IPassword Generate()
		{
			var clear = RandomPassword.Generate(PasswordCharactersCount);
			var salt = cryptoService.GenerateSalt();
			var hash = cryptoService.Compute(clear, salt);
			return new Password(){Clear = clear, Hash = hash, Salt = salt };
		}

		public bool IsPasswordValid(User user, string clear)
		{
			if (user == null || string.IsNullOrWhiteSpace(user.Salt) || string.IsNullOrWhiteSpace(user.Hash) || string.IsNullOrWhiteSpace(clear)) return false;
			return cryptoService.Compute(clear, user.Salt) == user.Hash;
		}

		public bool IsForgotPasswordTokenValid(User user, string token)
		{
			if (user == null || string.IsNullOrWhiteSpace(token) || !token.IsGuid()) return false;
			if (user.ForgotPasswordRequestToken != token) return false;

			var minutesElapsedSinceTokenRequested = (DateTime.Now - user.ForgotPasswordRequestDate).TotalMinutes;
			return minutesElapsedSinceTokenRequested < ForgotPasswordTimeLimitMinutes;
		}

	}
}
