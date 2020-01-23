// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrantsViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The grants view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Grants
{
    #region Usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The grants view model.
    /// </summary>
    public class GrantsViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the grants.
        /// </summary>
        public IEnumerable<GrantViewModel> Grants { get; set; }

        #endregion
    }

    /// <summary>
    /// The grant view model.
    /// </summary>
    public class GrantViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the api grant names.
        /// </summary>
        public IEnumerable<string> ApiGrantNames { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client logo url.
        /// </summary>
        public string ClientLogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the client url.
        /// </summary>
        public string ClientUrl { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the expires.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets the identity grant names.
        /// </summary>
        public IEnumerable<string> IdentityGrantNames { get; set; }

        #endregion
    }
}