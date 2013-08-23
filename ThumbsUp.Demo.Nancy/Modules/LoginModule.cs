using Nancy;
using Nancy.Authentication.Forms;
using ThumbsUp.Nancy.FormsAuthentication;

namespace ThumbsUp.Demo.Nancy.Module
{
	public class LoginModule : _BaseModule
	{
		public LoginModule(IThumbsUpNancyApi thumbsUp)
		{
			Get["/login"] = _ =>
			{
				return View["loginView"];
			};

			Get["/logout"] = url =>
			{
				var user = Context.CurrentUser as ThumbsUpNancyUser;
				if (user != null) thumbsUp.Logout(user.ThumbKey);
				return this.LogoutAndRedirect("~/");
			};

			Post["/login"] = _ =>
			{
				var result = thumbsUp.ValidateUser(Params.UserName, Params.Password);
				if (result.Success) return this.LoginAndRedirect(result.Data.ThumbKey.Value);
				
				SetMessageError(result.GetMessage());
				return View["loginView"];
			};

			Get["/password-request"] = _ =>
			{
				return View["PasswordRequestView"];
			};

			Post["/password-request"] = _ =>
			{
				var result = thumbsUp.ForgotPasswordRequest(Params.UserName, Params.Email);
				if (result.Success)
				{
					var token = result.Data.Token;
					var body = "To complete the new password request please click on the following link : " + Request.Url.SiteBase + "/password-generate/" + token;
					var sendResult = Gmail.Send(Params.Email, "ThumbsUp - Nancy Demo : New Password Request", body);
					var success = sendResult.Item1;
					if (success)
					{
						SetMessageSuccess("Success. Please check your email for further instructions.");
					}
					else
					{
						SetMessageError(sendResult.Item2);
					}
				}
				else
				{
					SetMessageError(result.GetMessage());
				}
				return View["PasswordRequestView"];
			};

			Get["/password-generate/{token}"] = _ =>
			{
				return View["PasswordGenerateView"];
			};

			Post["/password-generate/{token}"] = _ =>
			{
				var result = thumbsUp.ForgotPasswordReset(Params.UserName, Params.Token);
				if (result.Success)
				{
					SetMessageSuccess("Success. Your new password is shown below");
					ViewBag.NewPassword = result.Data.Password;
				}
				else 
				{ 
					SetMessageError(result.GetMessage());
				}
				return View["PasswordGenerateView"];
			};

		}

	}
}


