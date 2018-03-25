// -----------------------------------------------------------------------
// <copyright file="Startup.Auth.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Owin;
    using Logic;
    using IdentityModel.Tokens;
    using Models;
    using Owin.Security;
    using Owin.Security.Cookies;
    using Owin.Security.OpenIdConnect;
    using PartnerCenter.Exceptions;
    using PartnerCenter.Models.Customers;
    using Providers;
    using Unity;

    /// <summary>
    /// Provides methods and properties used to start the application.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures authentication for the application.
        /// </summary>
        /// <param name="app">The application to be configured.</param>
        public void ConfigureAuth(IAppBuilder app)
        {
            IExplorerProvider provider = UnityConfig.Container.Resolve<IExplorerProvider>();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = provider.Configuration.ApplicationId,
                    Authority = $"{provider.Configuration.ActiveDirectoryEndpoint}/common",

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = (context) =>
                        {
                            // Track the exceptions using the telemetry provider.
                            provider.Telemetry.TrackException(context.Exception);

                            // Pass in the context back to the app
                            context.OwinContext.Response.Redirect($"/Home/Error?message={context.Exception.Message}");

                            // Suppress the exception
                            context.HandleResponse();

                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            string userTenantId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signedInUserObjectId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

                            IGraphClient client = new GraphClient(provider, userTenantId);

                            List<RoleModel> roles = await client.GetDirectoryRolesAsync(signedInUserObjectId).ConfigureAwait(false);

                            foreach (RoleModel role in roles)
                            {
                                context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role.DisplayName));
                            }

                            bool isPartnerUser = userTenantId.Equals(
                                provider.Configuration.ApplicationTenantId, StringComparison.CurrentCultureIgnoreCase);

                            string customerId = string.Empty;

                            if (!isPartnerUser)
                            {
                                try
                                {
                                    Customer c = await provider.PartnerOperations.GetCustomerAsync(userTenantId).ConfigureAwait(false);
                                    customerId = c.Id;
                                }
                                catch (PartnerException ex)
                                {
                                    if (ex.ErrorCategory != PartnerErrorCategory.NotFound)
                                    {
                                        throw;
                                    }
                                }
                            }

                            if (isPartnerUser || !string.IsNullOrWhiteSpace(customerId))
                            {
                                // Add the customer identifier to the claims
                                context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("CustomerId", userTenantId));
                            }
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            return Task.FromResult(0);
                        }
                    },
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        SaveSigninToken = true,
                        ValidateIssuer = false
                    }
                });
        }
    }
}