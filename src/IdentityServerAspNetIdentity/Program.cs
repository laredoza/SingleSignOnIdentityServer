// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity
{
    #region Usings

    using System.Linq;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.SystemConsole.Themes;

    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Public Methods And Operators

        /// <summary>
        /// The create web host builder.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IWebHostBuilder"/>.
        /// </returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog(
                (context, configuration) =>
                    {
                        configuration.MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning).MinimumLevel
                            .Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information).Enrich
                            .FromLogContext().WriteTo.File(@"identityserver4_log.txt").WriteTo.Console(
                                outputTemplate:
                                "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                theme: AnsiConsoleTheme.Literate);
                    });
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var seed = args.Any(x => x == "/seed");
            if (seed) args = args.Except(new[] { "/seed" }).ToArray();

            var host = CreateWebHostBuilder(args).Build();

            if (seed)
            {
                var config = host.Services.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("DefaultConnection");
                // SeedData.EnsureSeedData(connectionString);
                return;
            }

            host.Run();
        }

        #endregion
    }
}