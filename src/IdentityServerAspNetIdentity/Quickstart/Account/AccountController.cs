// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The account controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    #region Usings

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Quickstart.UI;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using SingleSignOn.IdentityServerAspNetIdentity.Models;

    #endregion

    /// <summary>
    /// The account controller.
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
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
        /// The _scheme provider.
        /// </summary>
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        /// <summary>
        /// The _sign in manager.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// The _user manager.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">
        /// The user manager.
        /// </param>
        /// <param name="signInManager">
        /// The sign in manager.
        /// </param>
        /// <param name="interaction">
        /// The interaction.
        /// </param>
        /// <param name="clientStore">
        /// The client store.
        /// </param>
        /// <param name="schemeProvider">
        /// The scheme provider.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._interaction = interaction;
            this._clientStore = clientStore;
            this._schemeProvider = schemeProvider;
            this._events = events;
        }

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        /// <param name="returnUrl">
        /// The return Url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await this.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return this.RedirectToAction(
                    "Challenge",
                    "External",
                    new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return this.View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="button">
        /// The button.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await this._interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await this._interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await this._clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return this.Redirect(model.ReturnUrl);
                }

                // since we don't have a valid context, then we just go back to the home page
                return this.Redirect("~/");
            }

            if (this.ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(
                                 model.Username,
                                 model.Password,
                                 model.RememberLogin,
                                 lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await this._userManager.FindByNameAsync(model.Username);
                    await this._events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    if (context != null)
                    {
                        if (await this._clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return this.Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (this.Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return this.Redirect(model.ReturnUrl);
                    }

                    if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return this.Redirect("~/");
                    }

                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }

                await this._events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                this.ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await this.BuildLoginViewModelAsync(model);
            return this.View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        /// <param name="logoutId">
        /// The logout Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await this.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await this.Logout(vm);
            }

            return this.View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await this.BuildLoggedOutViewModelAsync(model.LogoutId);

            if (this.User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await this._signInManager.SignOutAsync();

                // raise the logout event
                await this._events.RaiseAsync(
                    new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = this.Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return this.SignOut(
                    new AuthenticationProperties { RedirectUri = url },
                    vm.ExternalAuthenticationScheme);
            }

            return this.View("LoggedOut", vm);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// The build logged out view model async.
        /// </summary>
        /// <param name="logoutId">
        /// The logout id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await this._interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
                         {
                             AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                             PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                             ClientName =
                                 string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                             SignOutIframeUrl = logout?.SignOutIFrameUrl,
                             LogoutId = logoutId
                         };

            if (this.User?.Identity.IsAuthenticated == true)
            {
                var idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await this.HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await this._interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/

        /// <summary>
        /// The build login view model async.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                           {
                               EnableLocalLogin = false,
                               ReturnUrl = returnUrl,
                               Username = context?.LoginHint,
                               ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                           };
            }

            var schemes = await this._schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(
                    x => x.DisplayName != null || (x.Name.Equals(
                                                          AccountOptions.WindowsAuthenticationSchemeName,
                                                          StringComparison.OrdinalIgnoreCase))).Select(
                    x => new ExternalProvider { DisplayName = x.DisplayName, AuthenticationScheme = x.Name }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await this._clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(
                                provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme))
                            .ToList();
                    }
                }
            }

            return new LoginViewModel
                       {
                           AllowRememberLogin = AccountOptions.AllowRememberLogin,
                           EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                           ReturnUrl = returnUrl,
                           Username = context?.LoginHint,
                           ExternalProviders = providers.ToArray()
                       };
        }

        /// <summary>
        /// The build login view model async.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await this.BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        /// <summary>
        /// The build logout view model async.
        /// </summary>
        /// <param name="logoutId">
        /// The logout id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (this.User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await this._interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        #endregion
    }
}