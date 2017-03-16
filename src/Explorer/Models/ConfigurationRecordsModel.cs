// -----------------------------------------------------------------------
// <copyright file="ConfigurationRecordsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model for domain configuration records obtain from Azure AD Graph API.
    /// </summary>
    public class ConfigurationRecordsModel
    {
        /// <summary>
        /// Gets or sets the service configuration records.
        /// </summary>
        public List<ServiceConfigurationRecordModel> ServiceConfigurationRecords { get; set; }
    }
}