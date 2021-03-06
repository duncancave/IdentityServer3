﻿namespace WebServer
{
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;

    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Services.InMemory;

    using Owin;

    using Serilog;

    using WebServer.Configuration;
    using WebServer.Services;

    public sealed class Startup
    {
        public Startup()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();
        }

        public void Configuration(IAppBuilder app)
        {
            // The OpenID Connect discovery endpoint is: https://localhost:44346/IdentityServer/.well-known/openid-configuration
            // Confirm the certificate has taken and is open to discovery: https://localhost:44346/IdentityServer/.well-known/jwks
            app.Map(
                "/IdentityServer",
                coreApp =>
                    {
                        var userService = new UserService();
                        var uriValidator = new UriValidator();
                        var requestValidator = new RequestValidator();

                        coreApp.UseIdentityServer(
                            new IdentityServerOptions
                                {
                                    SiteName = "Standalone Identity Server",
                                    SigningCertificate = this.LoadCertificate(),
                                    Factory = new IdentityServerServiceFactory
                                                  {
                                                      ClientStore = new Registration<IClientStore>(r => new InMemoryClientStore(Clients.Get())),
                                                      UserService = new Registration<IUserService>(r => userService),
                                                      ScopeStore = new Registration<IScopeStore>(r => new InMemoryScopeStore(Scopes.Get())),
                                                      RedirectUriValidator = new Registration<IRedirectUriValidator>(r => uriValidator),
                                                      CustomRequestValidator = new Registration<ICustomRequestValidator>(r => requestValidator),
                                                      ViewService = new Registration<IViewService, CustomViewService>()
                                                  },
                                    RequireSsl = true
                                });
                    });
        }

        // Using Certificates in Azure Websites Applications: https://azure.microsoft.com/en-us/blog/using-certificates-in-azure-websites-applications/
        X509Certificate2 LoadCertificate()
        {
            return
                new X509Certificate2(
                    string.Format(@"{0}\bin\Certificate\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory),
                    "idsrv3test");
        }
    }
}