// -----------------------------------------------------------------------
// <copyright file="IConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration
{
    /// <summary>
    /// Represents the ability to reference various configurations. 
    /// </summary>
    public interface IConfiguration
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
        /// Gets the application tenant identifier.
        /// </summary>
        string ApplicationTenantId { get; }

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        string ApplicationSecret { get; }

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
        /// Gets a value indicating whether or not the reseller tenant is the TIP tenant.
        /// </summary>
        bool IsIntegrationSandbox { get; }

        /// <summary>
        /// Gets the Office 365 Management endpoint address.
        /// </summary>
        string OfficeManagementEndpoint { get; }

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        string PartnerCenterApplicationId { get; }

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        string PartnerCenterApplicationSecret { get; }

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        string PartnerCenterApplicationTenantId { get; }

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        string PartnerCenterEndpoint { get; }

        /// <summary>
        /// Gets the Redis Cache connection string.
        /// </summary>
        string RedisCacheConnectionString { get; }

        /// <summary>
        /// Gets the vault application identifier.
        /// </summary>
        string VaultApplicationId { get; }

        /// <summary>
        /// Gets the vault application certificate thumbprint.
        /// </summary>
        string VaultApplicationCertThumbprint { get; }

        /// <summary>
        /// Gets the vault application tenant identifier.
        /// </summary>
        string VaultApplicationTenantId { get; }

        /// <summary>
        /// Gets the vault base address.
        /// </summary>
        string VaultBaseAddress { get; }
    }
}