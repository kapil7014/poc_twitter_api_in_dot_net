using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

namespace MissionG3_TwitterWeb_API.api
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }
        private IHostingEnvironment HostingEnvironment { get; set; }
        public IConfigurationRoot Configuration { get; }
        private string CurrentURL { get; set; }
        private string ConnectionString
        {
            get
            {
                return this.HostingEnvironment.IsDevelopment() ? Configuration.GetConnectionString("DefaultConnection") : Configuration.GetConnectionString("DefaultConnection");
            }
        }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            this.HostingEnvironment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Add the whole configuration object here.
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMvc()
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddDistributedMemoryCache();
            services.AddSession();
                        
            // create a Autofac container builder
            var builder = new ContainerBuilder();

            // read service collection to Autofac
            builder.Populate(services);

            builder.RegisterType<IFormFile>()
                .AsImplementedInterfaces().InstancePerDependency();

            // build the Autofac container
            ApplicationContainer = builder.Build();

            // creating the IServiceProvider out of the Autofac container
            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule());
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {               
            }

            //Cors
            app.UseCors(builder =>
            {
                var corsDomains = Configuration
                    .GetSection("Cors:AllowedDomains")?
                    .GetChildren()?
                    .Select(s => s.Value)
                    .ToArray();

                builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(corsDomains)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Web Socket 
            app.UseWebSockets();

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
        }

    }
}