using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace AspNet.ServerBrowser
{
    public class Startup
    {
        public void ConfigureServices(
            IServiceCollection services)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(path));
            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}