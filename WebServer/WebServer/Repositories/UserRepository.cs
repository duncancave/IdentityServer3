namespace WebServer.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    using IdentityServer3.Core;

    using WebServer.Classes;
    using WebServer.Interfaces;
    public class UserRepository : IUserRepository
    {
        private List<CustomUser> users = new List<CustomUser>{
                                                       new CustomUser
                                                           {
                                                               Subject = "1234",
                                                               Tenant = "Cheese",
                                                               Username = "Duncan",
                                                               Password = "Password1",
                                                               Claims =
                                                                   new List<Claim>
                                                                       {
                                                                           new Claim(Constants.ClaimTypes.GivenName, "Duncan"),
                                                                           new Claim(Constants.ClaimTypes.FamilyName, "Cave"),
                                                                           new Claim(Constants.ClaimTypes.Email, "duncancave@moneydebtandcredit.com"),
                                                                           new Claim(Constants.ClaimTypes.Role, "Hero")
                                                                       }
                                                           }
                                                   };

        //public List<CustomUser> GetUsers()
        //{
        //    return this.users;
        //}

        public CustomUser GetUser(string tenant, string username, string password)
        {
            var user = this.users.SingleOrDefault(x => x.Tenant.ToUpper() == tenant.ToUpper() && x.Username == username && x.Password == password);
            return user;
        }

        public CustomUser GetUserById(string Id)
        {
            var user = this.users.SingleOrDefault(x => x.Subject == Id);
            return user;
        }
    }
}