// -----------------------------------------------------------------------
// <copyright file="SubscriptionManageModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using Microsoft.Store.PartnerCenter.Explorer.Subscriptions;

    /// <summary>
    /// Model for managing subscriptions.
    /// </summary>
    public class SubscriptionManageModel
    {
        /// <summary>
        /// Gets or sets the name of the company who owns the subscription.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier assigned to the customer who owns the subscription.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the friendly of the subscription.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the subscription details.
        /// </summary>
        public ISubscriptionDetails SubscriptionDetails { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the type of the subscription.
        /// </summary>
        public string ViewName { get; set; }
    }
}