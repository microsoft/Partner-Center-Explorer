// -----------------------------------------------------------------------
// <copyright file="OfficeHealthEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Office
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Health event returned from Office 365 Service Communications API.
    /// </summary>
    public class OfficeHealthEvent : IHealthEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeHealthEvent" /> class.
        /// </summary>
        public OfficeHealthEvent()
        {
            IncidentIds = new List<string>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the type of the <see cref="IHealthEvent" />.
        /// </summary>
        public HealthEventType EventType => HealthEventType.Office;

        /// <summary>
        /// Gets or sets a list of incidents.
        /// </summary>
        public List<string> IncidentIds { get; }

        /// <summary>
        /// Gets or sets the status of the <see cref="IHealthEvent" />.
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