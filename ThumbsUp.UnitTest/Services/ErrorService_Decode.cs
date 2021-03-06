﻿#region Using
using Shouldly;
using ThumbsUp.Service;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ErrorService_Decode
	{
		[Fact]
		public void Should_return_correct_error_message_for_the_requested_error_code()
		{
			// Given
			var errorService = new ErrorService();
			var code = ((int)ErrorCode.UserNameTaken).ToString();

			// When
			var message = errorService.Decode(code);
			
			// Then
			message.ShouldBe("The UserName has already been taken");
		}

		[Fact]
		public void Should_return_InternalError_if_error_code_is_integer_but_unknown()
		{
			// Given
			var errorService = new ErrorService();
			var code = "-1";

			// When
			var message = errorService.Decode(code);
			
			// Then
			message.ShouldBe("Unrecognised internal error");
		}

		[Fact]
		public void Should_return_InternalError_if_error_code_is_not_valid_integer()
		{
			// Given
			var errorService = new ErrorService();
			var code = "<invalid-code>";

			// When
			var message = errorService.Decode(code);
			
			// Then
			message.ShouldBe("Unrecognised internal error");
		}
	}
}
