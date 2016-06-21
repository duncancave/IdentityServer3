namespace WebServer.Services
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

    public class UserService : IUserService
    {
        private IUserRepository repository;

        public UserService()
        {
            this.repository = new UserRepository();
        }

        public Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            //var user = Users.SingleOrDefault(x => x.Username == context.UserName && x.Password == context.Password);
            var user = this.repository.GetUser(context.SignInMessage.Tenant, context.UserName, context.Password);
            if (user != null)
            {
                context.AuthenticateResult = new AuthenticateResult(user.Subject, user.Username);
            }

            return Task.FromResult(0);
        }

        public Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            return Task.FromResult(0);
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // issue the claims for the user
            //var user = Users.SingleOrDefault(x => x.Subject == context.Subject.GetSubjectId());
            var user = this.repository.GetUserById(context.Subject.GetSubjectId());
            if (user != null)
            {
                // TODO: Add claims for user, tenant and publisher here
                context.IssuedClaims = user.Claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));
            }

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}