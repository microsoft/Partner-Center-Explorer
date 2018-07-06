// -----------------------------------------------------------------------
// <copyright file="OffersModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model for available offers from Partner Center.
    /// </summary>
    public class OffersModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffersModel" /> class.
        /// </summary>
        public OffersModel()
        {
            AvailableOffers = new List<OfferModel>();
        }

        /// <summary>
        /// Gets the available offers from Partner Center.
        /// </summary>
        public List<OfferModel> AvailableOffers { get; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }
    }
}