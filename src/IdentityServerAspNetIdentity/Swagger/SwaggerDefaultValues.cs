// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerDefaultValues.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Swagger
{
    #region Usings

    using System.Linq;

    using Microsoft.AspNetCore.Mvc.ApiExplorer;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    #endregion

    /// <summary>
    ///     Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>
    ///     This <see cref="IOperationFilter" /> is only required due to bugs in the <see cref="SwaggerGenerator" />.
    ///     Once they are fixed and published, this class can be removed.
    /// </remarks>
    public class SwaggerDefaultValues : IOperationFilter
    {
        #region Public Methods And Operators

        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">
        /// The operation to apply the filter to.
        /// </param>
        /// <param name="context">
        /// The current operation filter context.
        /// </param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Default == null)
                {
                    parameter.Default = description.DefaultValue;
                }

                parameter.Required |= description.IsRequired;
            }
        }

        #endregion
    }
}