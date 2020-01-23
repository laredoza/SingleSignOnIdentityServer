// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart
{
    #region Usings

    using System.Threading.Tasks;

    using IdentityServer4.Stores;

    #endregion

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods And Operators

        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">
        /// The store.
        /// </param>
        /// <param name="client_id">
        /// The client identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
        }

        #endregion
    }
}