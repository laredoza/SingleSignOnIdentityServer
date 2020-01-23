// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The error view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Home
{
    #region Usings

    using IdentityServer4.Models;

    #endregion

    /// <summary>
    /// The error view model.
    /// </summary>
    public class ErrorViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public ErrorMessage Error { get; set; }

        #endregion
    }
}