// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The login view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel : LoginInputModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether allow remember login.
        /// </summary>
        public bool AllowRememberLogin { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether enable local login.
        /// </summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// The external login scheme.
        /// </summary>
        public string ExternalLoginScheme =>
            this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        /// <summary>
        /// Gets or sets the external providers.
        /// </summary>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

        /// <summary>
        /// The is external login only.
        /// </summary>
        public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders?.Count() == 1;

        /// <summary>
        /// The visible external providers.
        /// </summary>
        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            this.ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        #endregion
    }
}