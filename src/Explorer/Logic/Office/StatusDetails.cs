// -----------------------------------------------------------------------
// <copyright file="StatusDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Office
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents status details obtained from the Office 365 Service Communication
    /// </summary>
    public class StatusDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the incident identifiers associated with this status.
        /// </summary>
        public List<string> IncidentIds { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the status time.
        /// </summary>
        public DateTime StatusTime { get; set; }

        /// <summary>
        /// Gets or sets the workload.
        /// </summary>
        public string Workload { get; set; }

        /// <summary>
        /// Gets or sets the display name of the workload.
        /// </summary>
        public string WorkloadDisplayName { get; set; }
    }
}