﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.DbContextFactory.Examples.Data.Persistence;
using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using EFCore.DbContextFactory.Extensions;

namespace EFCore.DbContextFactory.IntegrationTest
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
            services.AddMvc();

            var dbLogger = new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

            services.AddDbContext<OrderContext>(builder =>
                builder.UseInMemoryDatabase("OrdersExample"), ServiceLifetime.Transient);
            
            services.AddDbContextFactory<OrderContext>(builder => builder
                .UseInMemoryDatabase("OrdersExample")
                .UseLoggerFactory(dbLogger));

            services.AddScoped<OrderRepositoryWithFactory, OrderRepositoryWithFactory>();
            services.AddScoped<OrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}