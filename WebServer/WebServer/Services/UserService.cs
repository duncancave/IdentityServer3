﻿namespace WebServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using IdentityServer3.Core;
    using IdentityServer3.Core.Extensions;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Services.Default;

    using WebServer.Interfaces;
    using WebServer.Repositories;

    public class UserService : UserServiceBase
    {
        private IUserRepository repository;

        public UserService()
        {
            this.repository = new UserRepository();
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var user = this.repository.GetUser(context.SignInMessage.Tenant, context.UserName, context.Password);

            if (user == null)
            {
                context.AuthenticateResult = new AuthenticateResult("Incorrect credentials");
            }
            else
            {
                var extraClaims = new List<Claim> { new Claim("tenant", user.Tenant) };
                context.AuthenticateResult = new AuthenticateResult(user.Subject, user.Username, extraClaims);
            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // issue the claims for the user
            var user = this.repository.GetUserById(context.Subject.GetSubjectId());
            if (user != null)
            {
                // TODO: Add claims for user, tenant and publisher here
                var userClaims = user.Claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                userClaims.Add(new Claim("tenant", user.Tenant));

                context.IssuedClaims = userClaims;
            }

            return Task.FromResult(0);
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}