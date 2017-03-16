// -----------------------------------------------------------------------
// <copyright file="ExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using Cache;
    using Configuration;
    using Security;
    using Telemetry;

    /// <summary>
    /// Provides access to the core services.
    /// </summary>
    public class ExplorerService : IExplorerService
    {
        /// <summary>
        /// Provides the ability to cache often used objects. 
        /// </summary>
        private static ICacheService cache;

        /// <summary>
        /// Provides the ability to access various configurations.
        /// </summary>
        private static IConfiguration configuration;

        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private static ICommunication communication;

        /// <summary>
        /// Provides the ability to perform various partner operations.
        /// </summary>
        private static IPartnerOperations partnerOperations;

        /// <summary>
        /// Provides the ability to track telemetry data.
        /// </summary>
        private static ITelemetryProvider telemetry;

        /// <summary>
        /// Provides the ability to manage access tokens.
        /// </summary>
        private static ITokenManagement tokenManagement;

        /// <summary>
        /// Provides the ability to securely access and store resources.
        /// </summary>
        private static IVaultService vault;

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        public ICacheService Cache => cache ?? (cache = new CacheService(this));

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        public IConfiguration Configuration => configuration ?? (configuration = new Configuration(this));

        /// <summary>
        /// Gets a reference to the communication.
        /// </summary>
        public ICommunication Communication => communication ?? (communication = new Communication(this));

        /// <summary>
        /// Gets a reference to the partner operations.
        /// </summary>
        public IPartnerOperations PartnerOperations => partnerOperations ?? (partnerOperations = new PartnerOperations(this));

        /// <summary>
        /// Gets the Partner Center service reference.
        /// </summary>
        public IAggregatePartner PartnerCenter => PartnerService.Instance.CreatePartnerOperations(
               this.TokenManagement.GetPartnerCenterAppOnlyCredentials(
               $"{this.Configuration.ActiveDirectoryEndpoint}/{this.Configuration.PartnerCenterApplicationTenantId}"));

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        public ITelemetryProvider Telemetry
        {
            get
            {
                if (telemetry != null)
                {
                    return telemetry;
                }

                if (string.IsNullOrEmpty(this.Configuration.InstrumentationKey))
                {
                    telemetry = new EmptyTelemetryProvider();
                }
                else
                {
                    telemetry = new ApplicationInsightsTelemetryProvider();
                }

                return telemetry;
            }
        }

        /// <summary>
        /// Gets the a reference to the token management service.
        /// </summary>
        public ITokenManagement TokenManagement => tokenManagement ?? (tokenManagement = new TokenManagement(this));

        /// <summary>
        /// Gets a reference to the vault service.
        /// </summary>
        public IVaultService Vault => vault ?? (vault = new VaultService(this));
    }
}