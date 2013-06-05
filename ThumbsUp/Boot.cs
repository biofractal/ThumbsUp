using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using SimpleCrypto;
using ThumbsUp.Raven;
using ThumbsUp.Service;

namespace ThumbsUp
{
	public class Boot : DefaultNancyBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			container.Register<RavenSessionProvider>().AsSingleton();
			container.Register<ICryptoService, PBKDF2>().AsSingleton();
			base.ConfigureRequestContainer(container, context);
		}

		protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
		{
			pipelines.BeforeRequest += (ctx) => {
				if(context.Request.Path=="/") return null;
				var applicationService = container.Resolve<ApplicationService>();
				var applicationId = (string)context.Request.Form.applicationid;
				if(applicationService.ApplicationDoesNotExist(applicationId))
				{
					return HttpStatusCode.Unauthorized;
				}
				return null;
			};

			pipelines.AfterRequest.AddItemToEndOfPipeline(
				(ctx) =>
				{
					var documentSessionProvider = container.Resolve<RavenSessionProvider>();
					if (!documentSessionProvider.SessionInitialized) return;
					var documentSession = documentSessionProvider.Get();
					if (ctx.Response.StatusCode != HttpStatusCode.InternalServerError)
					{
						documentSession.SaveChanges();
					}
					documentSession.Dispose();
				});
		}
	}
}
