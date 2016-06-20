namespace WebApi.Controllers
{
    using System.Security.Claims;
    using System.Web.Http;

    [Route("test")]
    public class TestController : ApiController
    {
        public IHttpActionResult Get()
        {
            var caller = this.User as ClaimsPrincipal;

            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                return
                    this.Json(
                        new
                            {
                                message = "OK user",
                                client = caller.FindFirst("client_id").Value,
                                subject = subjectClaim.Value
                            });
            }

            return this.Json(new { message = "OK computer", client = caller.FindFirst("client_id").Value });
        }
    }
}
