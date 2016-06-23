namespace HybridClient.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using IdentityModel.Client;

    using Microsoft.Owin.Security;

    public class AccountController : Controller
    {
        private const string IdServBaseUri = @"https://localhost:44346/IdentityServer";
        private const string UserInfoEndpoint = IdServBaseUri + @"/connect/userinfo";
        private const string TokenEndpoint = IdServBaseUri + @"/connect/token";

        [Authorize]
        public ActionResult SignIn()
        {
            return this.Redirect("/");
        }

        public ActionResult SignOut()
        {
            this.Request.GetOwinContext().Authentication.SignOut();
            return this.Redirect("/");
        }

        // The calling code should check the expiry claim before refreshing the access token
        public async Task<ActionResult> RefreshAccessToken(string returnUrl)
        {
            var claimsPrincipal = this.User as ClaimsPrincipal;

            var tokenClient = new TokenClient(TokenEndpoint, "hybridclient", "idsrv3test");
            var response = await tokenClient.RequestRefreshTokenAsync(claimsPrincipal.FindFirst("refresh_token").Value);

            var manager = this.HttpContext.GetOwinContext().Authentication;

            var refreshedIdentity = new ClaimsIdentity(this.User.Identity);
            refreshedIdentity.RemoveClaim(refreshedIdentity.FindFirst("access_token"));
            refreshedIdentity.RemoveClaim(refreshedIdentity.FindFirst("refresh_token"));

            refreshedIdentity.AddClaim(new Claim("access_token", response.AccessToken));
            refreshedIdentity.AddClaim(new Claim("refresh_token", response.RefreshToken));

            // Replace the cookie with the new values
            manager.AuthenticationResponseGrant = new AuthenticationResponseGrant(
                new ClaimsPrincipal(refreshedIdentity),
                new AuthenticationProperties { IsPersistent = true });

            return this.Redirect(returnUrl);
        }
    }
}