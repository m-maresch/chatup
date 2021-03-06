﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Modules;
using NotificationService.Repositories;
using StackExchange.Redis;

namespace NotificationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var signalrCacheConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { Startup.Configuration["signalrCacheUrl"] }
            };

            services.AddSignalR()
                .AddRedis(options => options.Factory = writer => ConnectionMultiplexer.Connect(signalrCacheConfigurationOptions, writer));

            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<DefaultModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IBusControl bus, UsersContext usersContext, NotificationHandler notificationHandler)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            usersContext.Database.Migrate();

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/notifications");
            });
            
            app.UseMvc();

            notificationHandler.Start();

            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            
            lifetime.ApplicationStopping
                .Register(() => busHandle.Stop());
        }
    }
}
