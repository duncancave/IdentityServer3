namespace WebServer.Interfaces
{
    using System.Collections.Generic;

    using WebServer.Classes;

    interface IUserRepository
    {
        //List<CustomUser> GetUsers();

        CustomUser GetUser(string tenant, string username, string password);

        CustomUser GetUserById(string Id);
    }
}
