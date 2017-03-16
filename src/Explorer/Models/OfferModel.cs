// -----------------------------------------------------------------------
// <copyright file="OfferModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using PartnerCenter.Models.Invoices;

    /// <summary>
    /// Model for offers available for the specific country from Partner Center.
    /// </summary>
    public class OfferModel
    {
        /// <summary>
        /// Gets or sets the billing type.
        /// </summary>
        public BillingType Billing { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this offer is add on.
        /// </summary>
        public bool IsAddOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this offer is available for purchase.
        /// </summary>
        public bool IsAvailableForPurchase { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity.
        /// </summary>
        public int MaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity.
        /// </summary>
        public int MinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the prerequisite offers for this offer.
        /// </summary>
        public IEnumerable<string> PrerequisiteOffers { get; set; }
    }
}