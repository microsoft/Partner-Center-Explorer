// -----------------------------------------------------------------------
// <copyright file="AzureHealthEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using System;

    /// <summary>
    /// A health event returned from Azure Insights.
    /// </summary>
    public class AzureHealthEvent : IHealthEvent
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description of the <see cref="AzureHealthEvent"/>.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the event timestamp.
        /// </summary>
        /// <value>
        /// The event timestamp value.
        /// </value>
        public DateTime EventTimestamp { get; set; }

        /// <summary>
        /// Gets the type of the <see cref="IHealthEvent" />.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public HealthEventType EventType => HealthEventType.Azure;

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        /// The resource identifier value.
        /// </value>
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource group.
        /// </summary>
        /// <value>
        /// The name of the resource group.
        /// </value>
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource provider.
        /// </summary>
        /// <value>
        /// The name of the resource provider.
        /// </value>
        public string ResourceProviderName { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        /// <value>
        /// The type of the resource.
        /// </value>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the status of the <see cref="IHealthEvent" />.
        /// </summary>
        /// <value>
        /// The status of the <see cref="IHealthEvent" />.
        /// </value>
        public string Status { get; set; }
    }
}