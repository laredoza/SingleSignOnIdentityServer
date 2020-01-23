// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticsController.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The diagnostics controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Diagnostics
{
    #region Usings

    using System.Linq;
    using System.Threading.Tasks;

    using IdentityServer4.Quickstart.UI;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <summary>
    /// The diagnostics controller.
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        #region Public Methods And Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IActionResult> Index()
        {
            var localAddresses = new[] { "127.0.0.1", "::1", this.HttpContext.Connection.LocalIpAddress.ToString() };
            if (!localAddresses.Contains(this.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return this.NotFound();
            }

            var model = new DiagnosticsViewModel(await this.HttpContext.AuthenticateAsync());
            return this.View(model);
        }

        #endregion
    }
}