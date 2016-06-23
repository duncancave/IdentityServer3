namespace WebServer.Configuration
{
    using System.Collections.Generic;

    using IdentityServer3.Core;
    using IdentityServer3.Core.Models;

    // TODO: Use a client store with EF backend: https://github.com/IdentityServer/IdentityServer3.EntityFramework
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
                       {
                           new Client
                               {
                                   ClientId = "implicitclient",
                                   ClientName = "Example Implicit Client",
                                   Enabled = true,
                                   Flow = Flows.Implicit,
                                   RequireConsent = true,
                                   AllowRememberConsent = true,
                                   RedirectUris =
                                       new List<string>
                                           {
                                               "https://localhost:44335/account/signInCallback"
                                           },
                                   PostLogoutRedirectUris =
                                       new List<string> { "https://localhost:44335/" },
                                   AllowedScopes =
                                       new List<string>
                                           {
                                               Constants.StandardScopes.OpenId,
                                               Constants.StandardScopes.Profile,
                                               Constants.StandardScopes.Email
                                           },
                                   AccessTokenType = AccessTokenType.Jwt
                               },
                           new Client
                               {
                                   ClientId = "hybridclient",
                                   ClientName = "Example Hybrid Client",
                                   ClientSecrets =
                                       new List<Secret> { new Secret("idsrv3test".Sha256()) },
                                   Enabled = true,
                                   Flow = Flows.Hybrid,
                                   RequireConsent = false,
                                   AllowRememberConsent = false,
                                   //RedirectUris = new List<string> { "https://cheese.servertest.local:44310/" },
                                   //PostLogoutRedirectUris =
                                   //    new List<string> { "https://cheese.servertest.local:44310/" },
                                   AllowedScopes =
                                       new List<string>
                                           {
                                               Constants.StandardScopes.OpenId,
                                               Constants.StandardScopes.Profile,
                                               Constants.StandardScopes.Email,
                                               Constants.StandardScopes.Roles,
                                               Constants.StandardScopes.OfflineAccess,
                                               "SomeApi"
                                           },
                                   AccessTokenType = AccessTokenType.Jwt
                               }
                       };
        }
    }
}