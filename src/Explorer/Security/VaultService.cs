// -----------------------------------------------------------------------
// <copyright file="VaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;
    using Azure.Services.AppAuthentication;
    using Logic;

    /// <summary>
    /// Provides a secure mechanism for retrieving and store information. 
    /// </summary>
    internal sealed class VaultService : IVaultService
    {
        /// <summary>
        /// Provides access to the application core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Error code returned when a secret is not found in the vault.
        /// </summary>
        private const string NotFoundErrorCode = "SecretNotFound";

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultService"/> class.
        /// </summary>
        /// <param name="provider">Provides access to the application core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is null.
        /// </exception>
        public VaultService(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        public async Task<SecureString> GetAsync(string identifier)
        {
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            SecretBundle bundle;

            identifier.AssertNotEmpty(nameof(identifier));

            try
            {
                executionTime = DateTime.Now;

                using (IKeyVaultClient client = GetAzureKeyVaultClient())
                {
                    try
                    {
                        bundle = await client.GetSecretAsync(service.Configuration.KeyVaultEndpoint, identifier).ConfigureAwait(false);
                    }
                    catch (KeyVaultErrorException ex)
                    {
                        if (ex.Body.Error.Code.Equals(NotFoundErrorCode, StringComparison.CurrentCultureIgnoreCase))
                        {
                            bundle = null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "Identifier", identifier }
                };

                service.Telemetry.TrackEvent("KeyVault/GetAsync", eventProperties, eventMetrics);

                return bundle?.Value.ToSecureString();
            }
            finally
            {
                bundle = null;
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Stores the specified value in the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be stored.</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public async Task StoreAsync(string identifier, SecureString value)
        {
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            identifier.AssertNotEmpty(nameof(identifier));
            value.AssertNotNull(nameof(value));

            try
            {
                executionTime = DateTime.Now;

                using (IKeyVaultClient client = GetAzureKeyVaultClient())
                {
                    await client.SetSecretAsync(
                        service.Configuration.KeyVaultEndpoint, identifier, value.ToUnsecureString()).ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Identifier", identifier }
                };

                service.Telemetry.TrackEvent("KeyVault/StoreAsync", eventProperties, eventMetrics);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Gets an aptly configured instance of the <see cref="KeyVaultClient"/> class.
        /// </summary>
        /// <returns>An aptly populated instane of the <see cref="KeyVaultClient"/> class.</returns>
        private static KeyVaultClient GetAzureKeyVaultClient()
        {
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));
        }
    }
}