#region Using

using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using ThumbsUp.Nancy.FormsAuthentication;

#endregion

namespace ThumbsUp.Demo.Nancy
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			container.Register<IThumbsUpNancyApi, ThumbsUpNancyApi>();

		}

		protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
		{
			base.RequestStartup(requestContainer, pipelines, context);

			var formsAuthConfiguration = new FormsAuthenticationConfiguration()
			{
				RedirectUrl = "~/login",
				UserMapper = requestContainer.Resolve<IUserMapper>(),
			};

			FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
			nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("img"));
		}
	}
}