// -----------------------------------------------------------------------
// <copyright file="IConfigurationProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Providers
{
    using System.Security;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the ability to reference various configurations. 
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets the Active Directory endpoint address.
        /// </summary>
        string ActiveDirectoryEndpoint { get; }

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        string ApplicationId { get; }

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        SecureString ApplicationSecret { get; }

        /// <summary>
        /// Gets the application tenant identifier.
        /// </summary>
        string ApplicationTenantId { get; }

        /// <summary>
        /// Gets the Azure Resource Manager endpoint address.
        /// </summary>
        string AzureResourceManagerEndpoint { get; }

        /// <summary>
        /// Gets the Microsoft Graph endpoint address.
        /// </summary>
        string GraphEndpoint { get; }

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        string InstrumentationKey { get; }

        /// <summary>
        /// Gets the endpoint address for the instance of Key Vault.
        /// </summary>
        string KeyVaultEndpoint { get; }

        /// <summary>
        /// Gets the Office 365 Management endpoint address.
        /// </summary>
        string OfficeManagementEndpoint { get; }

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        string PartnerCenterAccountId { get; }

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        string PartnerCenterApplicationId { get; }

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        SecureString PartnerCenterApplicationSecret { get; }

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        string PartnerCenterEndpoint { get; }

        /// <summary>
        /// Gets the Redis Cache connection string.
        /// </summary>
        SecureString RedisCacheConnectionString { get; }

        /// <summary>
        /// Performs the necessary initialization operations.
        /// </summary>
        /// <returns>An instance of the  <see cref="Task"/> class that represents the asynchronous operation.</returns>
        Task InitializeAsync();
    }
}