using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DataAccess;
using DataAccess.Abstract;
using DataAccess.Repositories;
using IAR.DispatcherAPI.Interfaces;
using IAR.DispatcherAPI.Methods;
using IAR.DispatcherAPI.Model;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entities.Tenant;
using IAR.DispatcherAPI.Models;
using NLog.Extensions.Logging;
using IAR.DispatcherAPI.Filters;
using IAR.DispatcherAPI.Infraestructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySQL.Data.EntityFrameworkCore;
using MySQL.Data.Entity.Extensions;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IAR.DispatcherAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

            // Add framework services.
            services.AddEntityFrameworkMySQL().AddDbContext<DispatchApidbContext>(); //options => options.UseMySQL(Configuration["Data:DefaultConnection:ConnectionString"])

            services.AddOptions();

            // Add MVC services to the services container.
            services.AddMvc(
                config =>
                {
                    config.Filters.Add(new GlobalFilter());
                });

            services.AddScoped<ApiAuthorizationFilter>();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));

            // Create the Autofac container builder.
            var builder = new ContainerBuilder();

            // Add any Autofac modules or registrations.
            builder.RegisterType<Processor>().As<IProcessor>();
            builder.RegisterType<DataAccessProcessor>().As<IDataAccessProcessor>();
            builder.RegisterType<DispatchConnectorService>().As<IDispatchConnectorService>();
            builder.RegisterType<SubscriberRepository>().As<ISubscriberRepository>();
            builder.RegisterType<DispatchMessageRepository>().As<IDispatchMessageRepository>();
            builder.RegisterType<SubscriberByOriginationDestinationRepository>().As<ISubscriberByOriginationDestinationRepository>();
            builder.RegisterType<DispatchDestinationAddressesRepository>().As<IDispatchDestinationAddressesRepository>();
            builder.RegisterType<DispatchUnmatchedMessagesRepository>().As<IDispatchUnmatchedMessagesRepository>();
            builder.RegisterType<SubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository>()
                .As<ISubscriberInfoBySubscriberIdsInUnmatchedDispatchRepository>();
            builder.RegisterType<DispatchCentersRepository>().As<IDispatchCentersRepository>();

            // Populate the services.
            builder.Populate(services);

            // Build the container.
            ApplicationContainer = builder.Build();


            // Automapper Configuration
            AutoMapperConfiguration.Configure();

            // Create and return the service provider.
            return new AutofacServiceProvider(ApplicationContainer);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<DispatchApidbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }
            app.UseStaticFiles();
            app.UseMultitenancy<AppTenant>();
            app.UseMvc();
        }
    }
}
