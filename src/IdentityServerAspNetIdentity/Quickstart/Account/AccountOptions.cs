// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountOptions.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The account options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    #region Usings

    using System;

    #endregion

    /// <summary>
    /// The account options.
    /// </summary>
    public class AccountOptions
    {
        #region Static Fields

        // specify the Windows authentication scheme being used
        /// <summary>
        /// The windows authentication scheme name.
        /// </summary>
        public static readonly string WindowsAuthenticationSchemeName =
            Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;

        /// <summary>
        /// The allow local login.
        /// </summary>
        public static bool AllowLocalLogin = true;

        /// <summary>
        /// The allow remember login.
        /// </summary>
        public static bool AllowRememberLogin = true;

        /// <summary>
        /// The automatic redirect after sign out.
        /// </summary>
        public static bool AutomaticRedirectAfterSignOut = false;

        // if user uses windows auth, should we load the groups from windows
        /// <summary>
        /// The include windows groups.
        /// </summary>
        public static bool IncludeWindowsGroups = false;

        /// <summary>
        /// The invalid credentials error message.
        /// </summary>
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";

        /// <summary>
        /// The remember me login duration.
        /// </summary>
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// The show logout prompt.
        /// </summary>
        public static bool ShowLogoutPrompt = true;

        #endregion
    }
}