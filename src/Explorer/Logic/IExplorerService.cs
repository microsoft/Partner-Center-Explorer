// -----------------------------------------------------------------------
// <copyright file="IExplorerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Threading.Tasks;
    using Cache;
    using Configuration;
    using Security;
    using Telemetry;

    /// <summary>
    /// Represents the core service that powers this application.
    /// </summary>
    public interface IExplorerService
    {
        /// <summary>
        /// Gets a reference to the token management service.
        /// </summary>
        IAccessTokenProvider AccessToken { get; }

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        ICacheService Cache { get; }

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets a reference to the communication service.
        /// </summary>
        ICommunication Communication { get; }

        /// <summary>
        /// Gets a reference to the partner operations.
        /// </summary>
        IPartnerOperations PartnerOperations { get; }

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        ITelemetryProvider Telemetry { get; }

        /// <summary>
        /// Gets a reference to the vault service. 
        /// </summary>
        IVaultService Vault { get; }

        /// <summary>
        /// Initializes this instance of the <see cref="ReportProvider"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task InitializeAsync();
    }
}