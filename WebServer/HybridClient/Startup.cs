namespace HybridClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Helpers;

    using IdentityModel.Client;
    using IdentityModel.Extensions;

    using Microsoft.IdentityModel.Protocols;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;

    using Owin;

    public class Startup
    {
        private const string ClientUri = @"https://cheese.servertest.local:44310/";
        private const string IdServBaseUri = @"https://localhost:44346/IdentityServer";
        private const string UserInfoEndpoint = IdServBaseUri + @"/connect/userinfo";
        private const string TokenEndpoint = IdServBaseUri + @"/connect/token";

        public void Configuration(IAppBuilder app)
        {
            // The UniqueClaimTypeIdentifier setting requires that all claims-based identities must return a unique value for that claim type. 
            // This adjusts the anti-CSRF protection to use the sub claim and is necessary as a result of setting our inbound claim type map like we have.
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";

            // By setting this to a new dictionary of string and string we are essentially clearing the mapper, allowing the JWT middleware to use the claim types 
            // that are sent by Identity Server, instead of it mapping them to the Microsoft XML schema types(for example we'll get name instead of 
            // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name).
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationType = "Cookies" });

            // The Katana middleware is in charge of detecting when the user needs to be redirected to the OpenID Connect Provider and handling the subsequent interactions. 
            // This middleware will then log us in to our application as a result.
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = "hybridclient",
                    Authority = IdServBaseUri,
                    //RedirectUri = ClientUri,
                    //PostLogoutRedirectUri = ClientUri,
                    ResponseType = "code id_token token",
                    Scope = "openid profile email roles offline_access",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    },
                    SignInAsAuthenticationType = "Cookies",
                    Notifications =
                        new OpenIdConnectAuthenticationNotifications
                        {
                            AuthorizationCodeReceived = async n =>
                            {
                                // Check the uri and get the tenant
                                var uri = $"{n.Request.Scheme}://{n.Request.Host}/";
                                var tenant = string.Empty;

                                if (this.ValidateUri(uri))
                                {
                                    tenant = this.Between(uri, "//", ".");
                                }
                                else
                                {
                                    // TODO: Some kind of error here?
                                }

                                // Here we are using the UserInfoClient helper from the IdentityModel library to retrieve the user's claims and then 
                                // creating a new ClaimsIdentity, keeping the AuthenticationType the same. We are then populating this identity with 
                                // any claims we want both from the existing ClaimsIdentity and then any claims we received from the userinfo endpoint. 
                                // We can then store these claims in our authentication cookie.
                                var userInfoClient = new UserInfoClient(new Uri(UserInfoEndpoint), n.ProtocolMessage.AccessToken);
                                var userInfoResponse = await userInfoClient.GetAsync();

                                var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                                identity.AddClaims(userInfoResponse.GetClaimsIdentity().Claims);

                                // To add support for long lived logins, we'll need get a refresh token from our authorization server. 
                                // This refresh token can then be used to obtain fresh access tokens when the current one becomes invalid or expires.
                                // Here we are using another helper class from the IdentityModel library, this time TokenClient. 
                                // This is used for simple handling of OAuth requests and is internally a wrapper around HttpClient. 
                                // By trading in our Authorization Code like this we receive a new access token and a refresh token along with it.
                                var tokenClient = new TokenClient(TokenEndpoint, "hybridclient", "idsrv3test");
                                var response = await tokenClient.RequestAuthorizationCodeAsync(n.Code, uri);

                                identity.AddClaim(new Claim("access_token", response.AccessToken));
                                identity.AddClaim(
                                    new Claim("expires_at", DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToLocalTime().ToString(CultureInfo.InvariantCulture)));
                                identity.AddClaim(new Claim("refresh_token", response.RefreshToken));

                                // To use the post logout redirect we should also supply the previously issued ID token in order to give the 
                                // OpenID Connect Provider some sort of idea about the current authenticated session.
                                identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                                // Test tenant code - here we would get the tenant from the url (unless there is a better way?)
                                //var tenant = DateTime.Now.Minute % 2 != 0 ? "Tenant1" : "Tenant2";
                                identity.AddClaim(new Claim("tenant", tenant));

                                n.AuthenticationTicket = new AuthenticationTicket(
                                    identity,
                                    n.AuthenticationTicket.Properties);
                            },
                            RedirectToIdentityProvider = n =>
                            {                                
                                var uri = $"{n.Request.Scheme}://{n.Request.Host}/";

                                if (this.ValidateUri(uri))
                                {
                                    n.ProtocolMessage.RedirectUri = uri;
                                    n.ProtocolMessage.PostLogoutRedirectUri = uri;                                   
                                }
                                else
                                {
                                    // TODO: Some kind of error here?
                                }

                                if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                                {
                                    //if (n.OwinContext.Authentication.User.FindFirst("tenant") != null)
                                    //{
                                    //    var tenant = n.OwinContext.Authentication.User.FindFirst("tenant").Value;
                                    //    n.ProtocolMessage.AcrValues = $"tenant:{tenant}";
                                    //}

                                    var tenant = this.Between(uri, "//", ".");
                                    n.ProtocolMessage.AcrValues = $"tenant:{tenant}";                                   
                                }

                                if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                                {
                                    var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
                                    n.ProtocolMessage.IdTokenHint = idTokenHint;
                                }

                                return Task.FromResult(0);
                            }
                        }
                });
        }

        public bool ValidateUri(string uri)
        {
            // TODO: Get the subdomains from the provider/tenant
            var uris = new List<string>
                           {
                               "https://cheese.servertest.local:44310/",
                               "https://fish.servertest.local:44310/"
                           };
            return uris.Contains(uri);
        }

        public string Between(string value, string a, string b)
        {
            var posA = value.IndexOf(a);
            var posB = value.IndexOf(b);

            if (posA == -1)
            {
                return string.Empty;
            }

            if (posB == -1)
            {
                return string.Empty;
            }

            var adjustedPosA = posA + a.Length;

            return adjustedPosA >= posB ? "" : value.Substring(adjustedPosA, posB - adjustedPosA);
        }
    }
}