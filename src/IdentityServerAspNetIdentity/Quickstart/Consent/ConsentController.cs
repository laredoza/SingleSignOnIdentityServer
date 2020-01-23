// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsentController.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   This controller processes the consent UI
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Consent
{
    #region Usings

    using System.Linq;
    using System.Threading.Tasks;

    using IdentityServer4;
    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Quickstart.UI;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account;

    #endregion

    /// <summary>
    ///     This controller processes the consent UI
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ConsentController : Controller
    {
        #region Fields

        /// <summary>
        /// The _client store.
        /// </summary>
        private readonly IClientStore _clientStore;

        /// <summary>
        /// The _events.
        /// </summary>
        private readonly IEventService _events;

        /// <summary>
        /// The _interaction.
        /// </summary>
        private readonly IIdentityServerInteractionService _interaction;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger<ConsentController> _logger;

        /// <summary>
        /// The _resource store.
        /// </summary>
        private readonly IResourceStore _resourceStore;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentController"/> class.
        /// </summary>
        /// <param name="interaction">
        /// The interaction.
        /// </param>
        /// <param name="clientStore">
        /// The client store.
        /// </param>
        /// <param name="resourceStore">
        /// The resource store.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            IEventService events,
            ILogger<ConsentController> logger)
        {
            this._interaction = interaction;
            this._clientStore = clientStore;
            this._resourceStore = resourceStore;
            this._events = events;
            this._logger = logger;
        }

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// The create scope view model.
        /// </summary>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <returns>
        /// The <see cref="ScopeViewModel"/>.
        /// </returns>
        public ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
                       {
                           Name = scope.Name,
                           DisplayName = scope.DisplayName,
                           Description = scope.Description,
                           Emphasize = scope.Emphasize,
                           Required = scope.Required,
                           Checked = check || scope.Required
                       };
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await this.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return this.View("Index", vm);
            }

            return this.View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await this.ProcessConsent(model);

            if (result.IsRedirect)
            {
                if (await this._clientStore.IsPkceClientAsync(result.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.View("Redirect", new RedirectViewModel { RedirectUrl = result.RedirectUri });
                }

                return this.Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                this.ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            if (result.ShowView)
            {
                return this.View("Index", result.ViewModel);
            }

            return this.View("Error");
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// The build view model async.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var request = await this._interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                var client = await this._clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    var resources = await this._resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        return this.CreateConsentViewModel(model, returnUrl, request, client, resources);
                    }

                    this._logger.LogError(
                        "No scopes matching: {0}",
                        request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                }
                else
                {
                    this._logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }
            else
            {
                this._logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return null;
        }

        /// <summary>
        /// The create consent view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="resources">
        /// The resources.
        /// </param>
        /// <returns>
        /// The <see cref="ConsentViewModel"/>.
        /// </returns>
        private ConsentViewModel CreateConsentViewModel(
            ConsentInputModel model,
            string returnUrl,
            AuthorizationRequest request,
            Client client,
            Resources resources)
        {
            var vm = new ConsentViewModel
                         {
                             RememberConsent = model?.RememberConsent ?? true,
                             ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                             ReturnUrl = returnUrl,
                             ClientName = client.ClientName ?? client.ClientId,
                             ClientUrl = client.ClientUri,
                             ClientLogoUrl = client.LogoUri,
                             AllowRememberConsent = client.AllowRememberConsent
                         };

            vm.IdentityScopes = resources.IdentityResources.Select(
                x => this.CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();
            vm.ResourceScopes = resources.ApiResources.SelectMany(x => x.Scopes).Select(
                x => this.CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();
            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
            {
                vm.ResourceScopes = vm.ResourceScopes.Union(
                    new[]
                        {
                            this.GetOfflineAccessScope(
                                vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess)
                                || model == null)
                        });
            }

            return vm;
        }

        /// <summary>
        /// The create scope view model.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <returns>
        /// The <see cref="ScopeViewModel"/>.
        /// </returns>
        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
                       {
                           Name = identity.Name,
                           DisplayName = identity.DisplayName,
                           Description = identity.Description,
                           Emphasize = identity.Emphasize,
                           Required = identity.Required,
                           Checked = check || identity.Required
                       };
        }

        /// <summary>
        /// The get offline access scope.
        /// </summary>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <returns>
        /// The <see cref="ScopeViewModel"/>.
        /// </returns>
        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
                       {
                           Name = IdentityServerConstants.StandardScopes.OfflineAccess,
                           DisplayName = ConsentOptions.OfflineAccessDisplayName,
                           Description = ConsentOptions.OfflineAccessDescription,
                           Emphasize = true,
                           Checked = check
                       };
        }

        /*****************************************/
        /* helper APIs for the ConsentController */
        /*****************************************/

        /// <summary>
        /// The process consent.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            // validate return url is still valid
            var request = await this._interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model?.Button == "no")
            {
                grantedConsent = ConsentResponse.Denied;

                // emit event
                await this._events.RaiseAsync(
                    new ConsentDeniedEvent(this.User.GetSubjectId(), request.ClientId, request.ScopesRequested));
            }

            // user clicked 'yes' - validate the data
            else if (model?.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                                         {
                                             RememberConsent = model.RememberConsent, ScopesConsented = scopes.ToArray()
                                         };

                    // emit event
                    await this._events.RaiseAsync(
                        new ConsentGrantedEvent(
                            this.User.GetSubjectId(),
                            request.ClientId,
                            request.ScopesRequested,
                            grantedConsent.ScopesConsented,
                            grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // communicate outcome of consent back to identityserver
                await this._interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                result.ClientId = request.ClientId;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ViewModel = await this.BuildViewModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        #endregion
    }
}