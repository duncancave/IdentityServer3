using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple
{
    using Microsoft.Owin.Hosting;

    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            // Logging
            Log.Logger =
                new LoggerConfiguration()
                    .WriteTo
                    .LiterateConsole(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                    .CreateLogger();

            // Hosting IdentityServer
            using (WebApp.Start<Startup>("http://localhost:5000"))
            {
                Console.WriteLine("server running...");
                Console.ReadLine();
            }
        }
    }
}
