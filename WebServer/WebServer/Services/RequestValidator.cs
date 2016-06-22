namespace WebServer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using IdentityServer3.Core.Services;
    using IdentityServer3.Core.Validation;

    public class RequestValidator : ICustomRequestValidator
    {
        // Here we intercept the authentication request to check to see if the user is already authenticated with a tenant.
        // For most users we will block them here because the rule is one tenant per user.  Sysdmin and certain providers have different rulings
        // and I think this should be controlled from here.
        public Task<AuthorizeRequestValidationResult> ValidateAuthorizeRequestAsync(ValidatedAuthorizeRequest request)
        {
            var tenant = this.GetTenant(request.Raw.Get("acr_values"));
            var subject = request.Subject;
            if (subject.Identity == null || !subject.Identity.IsAuthenticated)
            {
                return Task.FromResult(new AuthorizeRequestValidationResult { IsError = false });
            }

            return Task.FromResult(this.IsUserAssociatedWithTenant(subject.Claims, tenant));
        }

        public Task<TokenRequestValidationResult> ValidateTokenRequestAsync(ValidatedTokenRequest request)
        {
            return Task.FromResult(new TokenRequestValidationResult { IsError = false });
        }

        private string GetTenant(string acr)
        {
            var pos = acr.IndexOf("tenant:");
            var length = "tenant:".Length;

            return acr.Substring(pos + length, acr.Length - length);
        }

        private AuthorizeRequestValidationResult IsUserAssociatedWithTenant(IEnumerable<Claim> claims, string tenant)
        {
            foreach (var claim in claims)
            {
                if (claim.Type == "tenant")
                {
                    if (claim.Value.ToUpper() == tenant.ToUpper())
                    {
                        return new AuthorizeRequestValidationResult { IsError = false };
                    }
                }
            }

            return new AuthorizeRequestValidationResult
                       {
                           ErrorType = ErrorTypes.User,
                           Error = $"User not authorised to access {tenant}",
                           ErrorDescription = $"User not authorised to access {tenant}",
                           IsError = true
                       };
        }
    }
}