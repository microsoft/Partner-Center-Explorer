// -----------------------------------------------------------------------
// <copyright file="ExplorerProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Providers
{
    using System.Threading.Tasks;
    using Logic;
    using Telemetry;

    /// <summary>
    /// Provides access to the core services.
    /// </summary>
    public class ExplorerProvider : IExplorerProvider
    {
        /// <summary>
        /// Provides the ability to manage access tokens.
        /// </summary>
        private static IAccessTokenProvider accessToken;

        /// <summary>
        /// Provides the ability to cache often used objects. 
        /// </summary>
        private static ICacheProvider cache;

        /// <summary>
        /// Provides the ability to access various configurations.
        /// </summary>
        private static IConfigurationProvider configuration;

        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private static IHttpService communication;

        /// <summary>
        /// Provides the ability to perform various partner operations.
        /// </summary>
        private static IPartnerOperations partnerOperations;

        /// <summary>
        /// Provides the ability to track telemetry data.
        /// </summary>
        private static ITelemetryProvider telemetry;

        /// <summary>
        /// Provides the ability to securely access and store resources.
        /// </summary>
        private static IVaultProvider vault;

        /// <summary>
        /// Gets the a reference to the token management service.
        /// </summary>
        public IAccessTokenProvider AccessToken => accessToken ?? (accessToken = new AccessTokenProvider(this));

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        public ICacheProvider Cache => cache ?? (cache = new RedisCacheProvider(this));

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        public IConfigurationProvider Configuration => configuration ?? (configuration = new ConfigurationProvider(this));

        /// <summary>
        /// Gets a reference to the communication.
        /// </summary>
        public IHttpService Communication => communication ?? (communication = new HttpService());

        /// <summary>
        /// Gets a reference to the partner operations.
        /// </summary>
        public IPartnerOperations PartnerOperations => partnerOperations ?? (partnerOperations = new PartnerOperations(this));

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

                if (string.IsNullOrEmpty(Configuration.InstrumentationKey))
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
        /// Gets a reference to the vault service.
        /// </summary>
        public IVaultProvider Vault => vault ?? (vault = new KeyVaultProvider(this));

        /// <summary>
        /// Initializes this instance of the <see cref="ReportProvider"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await Configuration.InitializeAsync().ConfigureAwait(false);
        }
    }
}