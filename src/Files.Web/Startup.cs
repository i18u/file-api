using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Files.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
					app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseExceptionHandler(new ExceptionHandlerOptions
			{
					ExceptionHandler = WriteExceptionToResponse
			});


			app.UseMvc();
		}

		private async Task WriteExceptionToResponse(HttpContext context)
		{
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
			if (ex == null) return;

			var error = new 
			{
				message = ex.Message
			};

			context.Response.ContentType = "application/json";

			using (var writer = new StreamWriter(context.Response.Body))
			{
				new JsonSerializer().Serialize(writer, error);
				await writer.FlushAsync().ConfigureAwait(false);
			}
		}

	}
}
