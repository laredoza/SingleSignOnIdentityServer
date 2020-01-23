// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoutViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The logout view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    /// <summary>
    /// The logout view model.
    /// </summary>
    public class LogoutViewModel : LogoutInputModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether show logout prompt.
        /// </summary>
        public bool ShowLogoutPrompt { get; set; } = true;

        #endregion
    }
}