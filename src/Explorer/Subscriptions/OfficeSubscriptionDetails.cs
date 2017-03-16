// -----------------------------------------------------------------------
// <copyright file="OfficeSubscriptionDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Subscriptions
{
    /// <summary>
    /// Represents subscription details for a license based subscription.
    /// </summary>
    public class OfficeSubscriptionDetails : ISubscriptionDetails
    {
        /// <summary>
        /// Gets or sets the friendly name of the subscription.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the quantity for the subscription.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription.
        /// </summary>
        public string Status { get; set; }
    }
}