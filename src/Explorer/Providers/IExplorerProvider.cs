// -----------------------------------------------------------------------
// <copyright file="IExplorerProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Providers
{
    using System.Threading.Tasks;
    using Logic;
    using Telemetry;

    /// <summary>
    /// Represents the core application provider.
    /// </summary>
    public interface IExplorerProvider
    {
        /// <summary>
        /// Gets a reference to the token management service.
        /// </summary>
        IAccessTokenProvider AccessToken { get; }

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        ICacheProvider Cache { get; }

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        IConfigurationProvider Configuration { get; }

        /// <summary>
        /// Gets a reference to the communication service.
        /// </summary>
        IHttpService Communication { get; }

        /// <summary>
        /// Gets a reference to the partner operations.
        /// </summary>
        IPartnerOperations PartnerOperations { get; }

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        ITelemetryProvider Telemetry { get; }

        /// <summary>
        /// Gets a reference to the vault provider. 
        /// </summary>
        IVaultProvider Vault { get; }

        /// <summary>
        /// Initializes this instance of the <see cref="ReportProvider"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task InitializeAsync();
    }
}