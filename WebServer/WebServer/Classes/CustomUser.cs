namespace WebServer.Classes
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class CustomUser
    {
        public string Subject { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Claim> Claims { get; set; }
    }
}