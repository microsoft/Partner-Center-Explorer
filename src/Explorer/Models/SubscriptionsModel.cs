// -----------------------------------------------------------------------
// <copyright file="SubscriptionsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model used to list subscriptions.
    /// </summary>
    public class SubscriptionsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsModel" /> class.
        /// </summary>
        public SubscriptionsModel()
        {
            Subscriptions = new List<SubscriptionModel>();
        }

        /// <summary>
        /// Gets the list of subscriptions owned by the customer.
        /// </summary>
        public List<SubscriptionModel> Subscriptions { get; }
    }
}