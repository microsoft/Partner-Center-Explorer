// -----------------------------------------------------------------------
// <copyright file="IncidentEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using System;

    /// <summary>
    /// Represents an incident event and used exclusively for querying. 
    /// </summary>
    public class IncidentEvent
    {
        /// <summary>
        /// Gets or sets the timestamp for the event.
        /// </summary>
        public DateTime EventTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the resource provider for the event.
        /// </summary>
        public string ResourceProvider { get; set; }
    }
}