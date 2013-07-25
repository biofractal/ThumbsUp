#region Using
using FakeItEasy;
using Nancy;
using Nancy.Helper;
using Shouldly;
using ThumbsUp.Service;
using ThumbsUp.Service.Domain;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ErrorService_Generate : _BaseServiceTest
	{
		[Fact]
		public void Should_return_valid_NancyResponse_containg_the_message_and_code_corresponding_to_the_requested_error()
		{
			// Given
			var errorService = new ErrorService();

			// When
			var dummyResponse = A.Dummy<IResponseFormatter>();
			var response = errorService.Generate(dummyResponse, ErrorCode.InternalError);
			
			// Then

		}
	}
}
