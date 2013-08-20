using Nancy;
using Nancy.Bootstrapper;
using Nancy.Helper;
using Nancy.TinyIoc;
using SimpleCrypto;
using ThumbsUp.Domain;

namespace ThumbsUp.Service
{
	public class ThumbsUpBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			container.Register<IRavenSessionProvider, RavenSessionProvider<RavenIndexes>>().AsSingleton();
			container.Register<ICryptoService, PBKDF2>();
			container.Register<IUserService, UserService>();
			container.Register<IUserCacheService, UserCacheService>();
			container.Register<IApplicationService, ApplicationService>();
			container.Register<IPasswordService, PasswordService>();
			container.Register<IErrorService, ErrorService>();
			base.ConfigureRequestContainer(container, context);
		}

		protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
		{
			pipelines.BeforeRequest += (ctx) =>
			{
				Log.Request(ctx.Request);
				var applicationService = container.Resolve<IApplicationService>();
				var applicationId = context.Request.GetParam("applicationid");
				if (!applicationService.IsRegistered(applicationId))
				{
					Log.Write("Warning: Unauthorized Access Attempt. ApplicationId = " + applicationId);
					return HttpStatusCode.Unauthorized;
				}
				return null;
			};

			pipelines.OnError += (ctx, ex) =>
			{
				Log.Error("Unhandled error on request: " + context.Request.Url, ex);
				return null;
			};

			pipelines.AfterRequest += (ctx) =>
			{
				if (ctx.Response.StatusCode == HttpStatusCode.OK) container.Resolve<IRavenSessionProvider>().SaveChangesAfterRequest();
				Log.Response(ctx.Response);
			};

			base.RequestStartup(container, pipelines, context);
		}
	}
}
