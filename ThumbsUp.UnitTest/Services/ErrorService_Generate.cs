#region Using
using FakeItEasy;
using Nancy;
using Nancy.Helper;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using ThumbsUp.Service.Domain;
using Xunit;
#endregion

namespace ThumbsUp.UnitTest.Services
{
	public class ErrorService_Generate : _BaseTest
	{
		[Fact]
		public void Should_return_valid_NancyResponse_containing_the_message_and_code_corresponding_to_the_requested_error()
		{
			// Given
			var errorService = new ErrorService();
			var formatter = new DefaultResponseFormatter(A.Dummy<IRootPathProvider>(), A.Dummy<NancyContext>(), A.Dummy<IEnumerable<ISerializer>>());
			var error = ErrorCode.UserNameTaken;

			// When
			var response = errorService.Generate(formatter, error);
			
			// Then
			response.ShouldNotBe(null);
			response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			var contents = response.GetContents();
			((object)contents).ShouldNotBe(null);
			((int)contents.ErrorCode).ShouldBe((int)error);
			((string)contents.ErrorMessage).ShouldBe("The UserName has already been taken");
		}

	}
}
