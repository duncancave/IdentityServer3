namespace WebServer.Configuration
{
    using System.Collections.Generic;

    using IdentityServer3.Core.Models;

    // TODO: Use a scope store with EF backend: https://github.com/IdentityServer/IdentityServer3.EntityFramework
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
                       {
                           StandardScopes.OpenId,
                           StandardScopes.Profile,
                           StandardScopes.Email,
                           StandardScopes.Roles,
                           StandardScopes.OfflineAccess
                       };
        }
    }
}