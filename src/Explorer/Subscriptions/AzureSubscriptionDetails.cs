// -----------------------------------------------------------------------
// <copyright file="AzureSubscriptionDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Subscriptions
{
    /// <summary>
    /// Represents status information for an Azure subscription.
    /// </summary>
    public class AzureSubscriptionDetails : ISubscriptionDetails
    {
        /// <summary>
        /// Gets or sets the friendly name of the subscription. 
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription.
        /// </summary>
        public string Status { get; set; }
    }
}