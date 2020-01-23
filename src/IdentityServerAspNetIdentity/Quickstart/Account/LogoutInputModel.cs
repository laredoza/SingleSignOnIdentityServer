// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoutInputModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The logout input model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    /// <summary>
    /// The logout input model.
    /// </summary>
    public class LogoutInputModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the logout id.
        /// </summary>
        public string LogoutId { get; set; }

        #endregion
    }
}