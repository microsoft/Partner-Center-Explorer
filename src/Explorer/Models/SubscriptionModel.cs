// -----------------------------------------------------------------------
// <copyright file="SubscriptionModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Subscriptions;

    /// <summary>
    /// Model for subscriptions managed though Partner Center.
    /// </summary>
    public class SubscriptionModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether automatic renewal is enabled.
        /// </summary>
        public bool AutoRenewEnabled { get; set; }

        /// <summary>
        /// Gets or sets the type of the billing.
        /// </summary>
        public BillingType BillingType { get; set; }

        /// <summary>
        /// Gets or sets the commitment end date.
        /// </summary>
        public DateTime CommitmentEndDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the effective start date.
        /// </summary>
        public DateTime EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the nick name of the subscription.
        /// </summary>
        [Display(Name = "Subscription Nickname")]
        [Required]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the name of the offer.
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the parent subscription identifier.
        /// </summary>
        public string ParentSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the partner identifier.
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the subscription status.
        /// </summary>
        [Required]
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the suspension reasons.
        /// </summary>
        public IEnumerable<string> SuspensionReasons { get; set; }

        /// <summary>
        /// Gets or sets the type of the units used by the subscription.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public string ViewModel { get; set; }
    }
}