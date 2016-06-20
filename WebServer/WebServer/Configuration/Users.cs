namespace WebServer.Configuration
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using IdentityServer3.Core;
    using IdentityServer3.Core.Services.InMemory;

    // TODO: Use a user service: https://identityserver.github.io/Documentation/docsv2/advanced/userService.html
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
                       {
                           new InMemoryUser
                               {
                                   Subject = "1",
                                   Username = "Scott Brady",
                                   Password = "Password123!",
                                   Claims =
                                       new List<Claim>
                                           {
                                               new Claim(
                                                   Constants.ClaimTypes
                                                   .GivenName,
                                                   "Scott"),
                                               new Claim(
                                                   Constants.ClaimTypes
                                                   .FamilyName,
                                                   "Brady"),
                                               new Claim(
                                                   Constants.ClaimTypes.Email,
                                                   "info@scottbrady91.com"),
                                               new Claim(
                                                   Constants.ClaimTypes.Role,
                                                   "Badmin")
                                           }
                               }
                       };
        }
    }
}