// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalProvider.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The external provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    /// <summary>
    /// The external provider.
    /// </summary>
    public class ExternalProvider
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the authentication scheme.
        /// </summary>
        public string AuthenticationScheme { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        #endregion
    }
}