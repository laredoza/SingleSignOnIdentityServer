// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsentInputModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The consent input model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Consent
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// The consent input model.
    /// </summary>
    public class ConsentInputModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember consent.
        /// </summary>
        public bool RememberConsent { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the scopes consented.
        /// </summary>
        public IEnumerable<string> ScopesConsented { get; set; }

        #endregion
    }
}