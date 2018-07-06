// -----------------------------------------------------------------------
// <copyright file="NewSubscriptionModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using PartnerCenter.Models.Orders;

    /// <summary>
    /// Model for creating new orders for Partner Center.
    /// </summary>
    public class NewSubscriptionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewSubscriptionModel" /> class.
        /// </summary>
        public NewSubscriptionModel()
        {
            AvailableOffers = new List<OfferModel>();
            LineItems = new List<OrderLineItem>();
        }
        
        /// <summary>
        /// Gets or sets the collection available offers.
        /// </summary>
        public List<OfferModel> AvailableOffers { get; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order line items.
        /// </summary>
        public List<OrderLineItem> LineItems { get; }
    }
}