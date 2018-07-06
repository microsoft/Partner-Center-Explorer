// -----------------------------------------------------------------------
// <copyright file="InvoiceDetailsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using PartnerCenter.Models.Invoices;

    /// <summary>
    /// Model for displaying invoice details.
    /// </summary>
    public class InvoiceDetailsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetailsModel" /> class.
        /// </summary>
        public InvoiceDetailsModel()
        {
            Customers = new List<string>();
            InvoiceLineItems = new List<InvoiceLineItem>();
        }

        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        public List<string> Customers { get; }

        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets the invoice line items.
        /// </summary>
        public List<InvoiceLineItem> InvoiceLineItems { get; }
    }
}