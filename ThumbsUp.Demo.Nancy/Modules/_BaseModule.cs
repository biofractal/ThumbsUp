#region Using

using Nancy;
using Nancy.ModelBinding;
using System.Configuration;
using System.Dynamic;

#endregion

namespace ThumbsUp.Demo.Nancy.Module
{
	public class _BaseModule : NancyModule
	{
		public class ParameterBag
		{
			
			public string UserName { get; set; }
			public string Password { get; set; }
			public string Email { get; set; }
			public string Token { get; set; }
			public string Name { get; set; }
			public string Id { get; set; }
		}

		public ParameterBag Params;
		public _BaseModule() : this("")
		{}

		public _BaseModule(string modulePath) : base(modulePath)
		{

			Before += ctx => 
			{			
				ViewBag.Title = "ThumbsUp Nancy - " + this.GetType().Name.Replace("Module", string.Empty);
				ViewBag.HasMessage = false;
				Params = this.Bind<ParameterBag>();
				return null; 
			};

		}

		protected void SetMessageError(string message)
		{
			SetMessage(message, false);
		}

		protected void SetMessageSuccess(string message)
		{
			SetMessage(message, true);
		}

		private void SetMessage(string message, bool isSuccess) 
		{
			ViewBag.HasMessage = true;
			ViewBag.Success = isSuccess;
			ViewBag.Message = message;
		}

	}
}
