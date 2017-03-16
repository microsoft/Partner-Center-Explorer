// -----------------------------------------------------------------------
// <copyright file="AuditRecordsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using PartnerCenter.Models;
    using PartnerCenter.Models.Auditing;

    /// <summary>
    /// View model for audit log records from Partner Center.
    /// </summary>
    public class AuditRecordsModel
    {
        /// <summary>
        /// Gets or sets the collection of audit log records.
        /// </summary>
        public SeekBasedResourceCollection<AuditRecord> Records { get; set; }
    }
}