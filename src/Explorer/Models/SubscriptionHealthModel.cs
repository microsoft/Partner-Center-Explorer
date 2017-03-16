// -----------------------------------------------------------------------
// <copyright file="SubscriptionHealthModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using Logic;

    /// <summary>
    /// Model for the subscription health view.
    /// </summary>
    public class SubscriptionHealthModel
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the subscription nick name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the health events.
        /// </summary>
        public List<IHealthEvent> HealthEvents { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public string ViewModel { get; set; }
    }
}