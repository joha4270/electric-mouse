using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace electric_mouse
{
    public class Program
    {
        private string dir = Directory.GetCurrentDirectory();
        X509Certificate2 cert = new X509Certificate2("textCert.pfx", "testPassword");

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.UseHttps("testCert.pfx", "testPassword");
                })
                .UseUrls("https://localhost:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
