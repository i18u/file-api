using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Files.Web
{
    public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
					.UseKestrel((config, options) => {
						if (config.HostingEnvironment.IsDevelopment()) {
							options.Listen(IPAddress.Loopback, 5000);
						} else {
							options.Listen(IPAddress.Any, 80);
						}
					})
					.UseStartup<Startup>();

	}
}
