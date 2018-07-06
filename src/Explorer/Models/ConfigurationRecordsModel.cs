// -----------------------------------------------------------------------
// <copyright file="ConfigurationRecordsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using Microsoft.Graph;

    /// <summary>
    /// Model for domain configuration records obtain from Azure AD Graph API.
    /// </summary>
    public class ConfigurationRecordsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRecordsModel" /> class.
        /// </summary>
        public ConfigurationRecordsModel()
        {
            ServiceConfigurationRecords = new List<DomainDnsRecord>();
        }

        /// <summary>
        /// Gets the service configuration records.
        /// </summary>
        public List<DomainDnsRecord> ServiceConfigurationRecords { get; }
    }
}