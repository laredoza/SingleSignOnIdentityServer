// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupExtension" company="">
//
// </copyright>
// <summary>
//   The class StartupExtension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Extensions
{
    #region Usings

    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SingleSignOn.Data.Context;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using SingleSignOn.IdentityServerAspNetIdentity.Swagger;
    using IdentityServer4.EntityFramework.Options;
    using SingleSignOn.IdentityServerAspNetIdentity.Models;
    using Microsoft.AspNetCore.Hosting;

    #endregion

    /// <summary>
    ///     The StartupExtension.
    /// </summary>
    public static class StartupExtension
    {
        #region Constructors and Destructors
        #endregion

        #region Public Properties


        #endregion

        #region Public Methods And Operators

        public static void AddApplicationContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                ConfigureDatabaseOptions(configuration, options);
            });
        }
        public static void AddApiVersioningAndSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning(
                options =>
                    {
                        // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                    });
            services.AddVersionedApiExplorer(
                options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";

                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                    {
                        // add a custom operation filter which sets default values
                        options.OperationFilter<SwaggerDefaultValues>();

                        // integrate xml comments
                        // options.IncludeXmlComments( XmlCommentsFilePath );
                    });
        }

        public static void AddIdentityServerSettings(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment)
        {
            var builder = services.AddIdentityServer(
                    options =>
                        {
                            options.Events.RaiseErrorEvents = true;
                            options.Events.RaiseInformationEvents = true;
                            options.Events.RaiseFailureEvents = true;
                            options.Events.RaiseSuccessEvents = true;
                        })
                .AddAspNetIdentity<ApplicationUser>().AddConfigurationStore(
                    options =>
                        {
                            ConfigureStoreOptions(configuration, options);
                        })

                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(
                    options =>
                        {
                            ConfigureOperationalOptions(configuration, options);
                            options.EnableTokenCleanup = true;
                        });

            if (environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                builder.AddDeveloperSigningCredential();

                // throw new Exception("need to configure key material");
            }
        }

        #endregion

        #region Other Methods

        private static void ConfigureDatabaseOptions(IConfiguration configuration, DbContextOptionsBuilder options)
        {
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configuration.GetValue<string>("DatabaseType"));
            switch (databaseType)
            {
                case DatabaseType.Postgres:
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MsSql:
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MySql:
                    throw new Exception("MySql Database not implemented");
                    break;
                default:
                    throw new Exception("Database not implemented");
                    break;
            }
        }

        private static void ConfigureStoreOptions(IConfiguration configuration, ConfigurationStoreOptions options)
        {
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configuration.GetValue<string>("DatabaseType"));

            switch (databaseType)
            {
                case DatabaseType.Postgres:
                    options.ConfigureDbContext = b => b.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MsSql:
                    options.ConfigureDbContext = b => b.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MySql:
                    throw new Exception("MySql Database not implemented");
                    break;
                default:
                    throw new Exception("Database not implemented");
                    break;
            }
        }

        private static void ConfigureOperationalOptions(IConfiguration configuration, OperationalStoreOptions options)
        {
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configuration.GetValue<string>("DatabaseType"));

            switch (databaseType)
            {
                case DatabaseType.Postgres:
                    options.ConfigureDbContext = b => b.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MsSql:
                    options.ConfigureDbContext = b => b.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    break;
                case DatabaseType.MySql:
                    throw new Exception("MySql Database not implemented");
                    break;
                default:
                    throw new Exception("Database not implemented");
                    break;
            }
        }

        #endregion
    }
}