namespace HybridClient.Controllers
{
    using System.Web;
    using System.Web.Mvc;

    public class AccountController : Controller
    {
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
    }
}