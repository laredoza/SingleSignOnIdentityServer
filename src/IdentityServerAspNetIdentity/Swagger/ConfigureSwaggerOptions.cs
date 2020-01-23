// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSwaggerOptions.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The configure swagger options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Swagger
{
    #region Usings

    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    #endregion

    /// <summary>
    /// The configure swagger options.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        #region Fields

        /// <summary>
        /// The provider.
        /// </summary>
        private readonly IApiVersionDescriptionProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in this.provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new Info
                        {
                            Title = $"Single Sign-on API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
            }
        }

        #endregion
    }
}