// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   The startup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity
{
    #region Usings

    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using SingleSignOn.Data.Context;
    using SingleSignOn.IdentityServerAspNetIdentity.Extensions;
    using SingleSignOn.IdentityServerAspNetIdentity.Models;
    using SingleSignOn.IdentityServerAspNetIdentity.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using SingleSignOn.IdentityServerAspNetIdentity.Extensions;

    #endregion

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="environment">
        /// The environment.
        /// </param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;

            var builder = new ConfigurationBuilder().SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile(
                    $"appsettings.{environment.EnvironmentName}.json",
                    optional: true).AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        public IHostingEnvironment Environment { get; }

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets the xml comments file path.
        /// </summary>
        string XmlCommentsFilePath
        {
            get
            {
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(this.Environment.ContentRootPath, fileName);
            }
        }

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            // this will do the initial DB population
            // Todo: This is a temp step.
            // SeedData.InitializeDatabase(app);
            // SeedData.EnsureSeedData(this.Configuration.GetConnectionString("DefaultConnection"));

            if (this.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(
                options =>
                    {
                        options.AllowAnyOrigin();
                        options.AllowAnyMethod();
                        options.AllowAnyHeader();
                    });

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                    {
                        // build a swagger endpoint for each discovered API version
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());
                        }
                    });
        }

        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationContext(this.Configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddMvc(options => options.EnableEndpointRouting = true)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.AddApiVersioningAndSwagger();

            services.Configure<IISOptions>(
                iis =>
                    {
                        iis.AuthenticationDisplayName = "Windows";
                        iis.AutomaticAuthentication = false;
                    });
            
            services.AddIdentityServerSettings(this.Configuration, this.Environment);
            
            services.AddAuthentication().AddGoogle(
                options =>
                    {
                        // register your IdentityServer with Google at https://console.developers.google.com
                        // enable the Google+ API
                        // set the redirect URI to http://localhost:5000/signin-google
                        options.ClientId = "copy client ID from Google here";
                        options.ClientSecret = "copy client secret from Google here";
                    });
        }

        #endregion
    }
}