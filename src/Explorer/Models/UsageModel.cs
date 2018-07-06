// -----------------------------------------------------------------------
// <copyright file="UsageModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using PartnerCenter.Models.Utilizations;

    /// <summary>
    /// Model for Azure subscription usage details.
    /// </summary>
    public class UsageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageModel" /> class.
        /// </summary>
        public UsageModel()
        {
            Usage = new List<AzureUtilizationRecord>();
        }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the subscription nick name.
        /// </summary>
        public string SubscriptionFriendlyName { get; set; }

        /// <summary>
        /// Gets the utilization records for the subscription.
        /// </summary>
        public List<AzureUtilizationRecord> Usage { get; }
    }
}