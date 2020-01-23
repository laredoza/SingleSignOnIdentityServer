// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceAuthorizationViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The device authorization view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Device
{
    using IdentityServer4.Quickstart.UI;

    /// <summary>
    /// The device authorization view model.
    /// </summary>
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether confirm user code.
        /// </summary>
        public bool ConfirmUserCode { get; set; }

        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        public string UserCode { get; set; }

        #endregion
    }
}