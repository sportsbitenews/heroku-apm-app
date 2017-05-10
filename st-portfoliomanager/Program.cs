using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SofttrendsAddon.Helpers;

namespace SofttrendsAddon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "development");
            string appEnv = Utilities.GetEnvVarVal("ASPNETCORE_ENVIRONMENT");
            string port = Utilities.GetEnvVarVal("PORT");
            if (!string.IsNullOrEmpty(appEnv) && appEnv.ToLower() == "development" && string.IsNullOrEmpty(port))
            {
                port = "5000";
            }
            else if (string.IsNullOrEmpty(port))
            {
                port = "5000";
            }
            Console.WriteLine(string.Format("Port listening on: {0}", port));

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(string.Format("http://*:{0}", port))  //Only production
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
