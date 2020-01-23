// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginInputModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The login input model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    #region Usings

    using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// The login input model.
    /// </summary>
    public class LoginInputModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember login.
        /// </summary>
        public bool RememberLogin { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        #endregion
    }
}