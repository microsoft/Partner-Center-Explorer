// -----------------------------------------------------------------------
// <copyright file="ISubscriptionDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Subscriptions
{
    /// <summary>
    /// Represents the status of a subscription.
    /// </summary>
    public interface ISubscriptionDetails
    {
        /// <summary>
        /// Gets or sets the friendly name of the subscription.
        /// </summary>
        string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription.
        /// </summary>
        string Status { get; set; }
    }
}