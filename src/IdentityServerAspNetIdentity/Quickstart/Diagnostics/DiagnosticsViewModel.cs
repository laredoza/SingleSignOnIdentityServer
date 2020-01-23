// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticsViewModel.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The diagnostics view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Diagnostics
{
    #region Usings

    using System.Collections.Generic;
    using System.Text;

    using IdentityModel;

    using Microsoft.AspNetCore.Authentication;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The diagnostics view model.
    /// </summary>
    public class DiagnosticsViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsViewModel"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public DiagnosticsViewModel(AuthenticateResult result)
        {
            this.AuthenticateResult = result;

            if (result.Properties.Items.ContainsKey("client_list"))
            {
                var encoded = result.Properties.Items["client_list"];
                var bytes = Base64Url.Decode(encoded);
                var value = Encoding.UTF8.GetString(bytes);

                this.Clients = JsonConvert.DeserializeObject<string[]>(value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the authenticate result.
        /// </summary>
        public AuthenticateResult AuthenticateResult { get; }

        /// <summary>
        /// Gets the clients.
        /// </summary>
        public IEnumerable<string> Clients { get; } = new List<string>();

        #endregion
    }
}