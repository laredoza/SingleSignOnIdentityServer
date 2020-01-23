// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The home controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Home
{
    #region Usings

    using System.Threading.Tasks;

    using IdentityServer4.Quickstart.UI;
    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// The home controller.
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        #region Fields

        /// <summary>
        /// The _environment.
        /// </summary>
        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// The _interaction.
        /// </summary>
        private readonly IIdentityServerInteractionService _interaction;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="interaction">
        /// The interaction.
        /// </param>
        /// <param name="environment">
        /// The environment.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public HomeController(
            IIdentityServerInteractionService interaction,
            IHostingEnvironment environment,
            ILogger<HomeController> logger)
        {
            this._interaction = interaction;
            this._environment = environment;
            this._logger = logger;
        }

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// Shows the error page
        /// </summary>
        /// <param name="errorId">
        /// The error Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await this._interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!this._environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return this.View("Error", vm);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Index()
        {
            if (this._environment.IsDevelopment())
            {
                // only show in development
                return this.View();
            }

            this._logger.LogInformation("Homepage is disabled in production. Returning 404.");
            return this.NotFound();
        }

        #endregion
    }
}