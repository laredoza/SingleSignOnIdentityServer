// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggedOutViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The logged out view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    /// <summary>
    /// The logged out view model.
    /// </summary>
    public class LoggedOutViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether automatic redirect after sign out.
        /// </summary>
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the external authentication scheme.
        /// </summary>
        public string ExternalAuthenticationScheme { get; set; }

        /// <summary>
        /// Gets or sets the logout id.
        /// </summary>
        public string LogoutId { get; set; }

        /// <summary>
        /// Gets or sets the post logout redirect uri.
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the sign out iframe url.
        /// </summary>
        public string SignOutIframeUrl { get; set; }

        /// <summary>
        /// The trigger external signout.
        /// </summary>
        public bool TriggerExternalSignout => this.ExternalAuthenticationScheme != null;

        #endregion
    }
}