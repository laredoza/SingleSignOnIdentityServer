// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalController.cs" company="">
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.   
// </copyright>
// <summary>
//   The external controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SingleSignOn.IdentityServerAspNetIdentity.Quickstart.Account
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4.Events;
    using IdentityServer4.Quickstart.UI;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using SingleSignOn.IdentityServerAspNetIdentity.Models;

    #endregion

    /// <summary>
    /// The external controller.
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
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
        /// Initializes a new instance of the <see cref="ExternalController"/> class.
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
        /// <param name="events">
        /// The events.
        /// </param>
        public ExternalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._interaction = interaction;
            this._clientStore = clientStore;
            this._events = events;
        }

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await this.FindUserFromExternalProviderAsync(result);
            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = await this.AutoProvisionUserAsync(provider, providerUserId, claims);
            }

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            this.ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            this.ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            this.ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await this._signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;
            await this._events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name));
            await this.HttpContext.SignInAsync(
                user.Id,
                name,
                provider,
                localSignInProps,
                additionalLocalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];
            if (this._interaction.IsValidReturnUrl(returnUrl) || this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.Redirect("~/");
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="returnUrl">
        /// The return Url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (this.Url.IsLocalUrl(returnUrl) == false && this._interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // windows authentication needs special handling
                return await this.ProcessWindowsLoginAsync(returnUrl);
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
                            {
                                RedirectUri = this.Url.Action(nameof(this.Callback)),
                                Items = {
                                           { "returnUrl", returnUrl }, { "scheme", provider } 
                                        }
                            };

            return this.Challenge(props, provider);
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// The auto provision user async.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="providerUserId">
        /// The provider user id.
        /// </param>
        /// <param name="claims">
        /// The claims.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private async Task<ApplicationUser> AutoProvisionUserAsync(
            string provider,
            string providerUserId,
            IEnumerable<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value
                       ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value
                            ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value
                           ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value
                        ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }

            var user = new ApplicationUser { UserName = Guid.NewGuid().ToString() };
            var identityResult = await this._userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await this._userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await this._userManager.AddLoginAsync(
                                 user,
                                 new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }

        /// <summary>
        /// The find user from external provider async.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)> FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject)
                              ?? externalUser.FindFirst(ClaimTypes.NameIdentifier)
                              ?? throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await this._userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        /// <summary>
        /// The process login callback for oidc.
        /// </summary>
        /// <param name="externalResult">
        /// The external result.
        /// </param>
        /// <param name="localClaims">
        /// The local claims.
        /// </param>
        /// <param name="localSignInProps">
        /// The local sign in props.
        /// </param>
        private void ProcessLoginCallbackForOidc(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        /// <summary>
        /// The process login callback for saml 2 p.
        /// </summary>
        /// <param name="externalResult">
        /// The external result.
        /// </param>
        /// <param name="localClaims">
        /// The local claims.
        /// </param>
        /// <param name="localSignInProps">
        /// The local sign in props.
        /// </param>
        private void ProcessLoginCallbackForSaml2p(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        /// <summary>
        /// The process login callback for ws fed.
        /// </summary>
        /// <param name="externalResult">
        /// The external result.
        /// </param>
        /// <param name="localClaims">
        /// The local claims.
        /// </param>
        /// <param name="localSignInProps">
        /// The local sign in props.
        /// </param>
        private void ProcessLoginCallbackForWsFed(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        /// <summary>
        /// The process windows login async.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await this.HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, treating windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties
                                {
                                    RedirectUri = this.Url.Action("Callback"),
                                    Items =
                                        {
                                            { "returnUrl", returnUrl },
                                            { "scheme", AccountOptions.WindowsAuthenticationSchemeName }
                                        }
                                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                // add the groups as claims -- be careful if the number of groups is too large
                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await this.HttpContext.SignInAsync(
                    IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return this.Redirect(props.RedirectUri);
            }

            // trigger windows auth
            // since windows auth don't support the redirect uri,
            // this URL is re-triggered when we call challenge
            return this.Challenge(AccountOptions.WindowsAuthenticationSchemeName);
        }

        #endregion
    }
}