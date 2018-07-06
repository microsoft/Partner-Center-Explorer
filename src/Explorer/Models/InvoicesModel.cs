// -----------------------------------------------------------------------
// <copyright file="InvoicesModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using PartnerCenter.Models.Invoices;

    /// <summary>
    /// Model for listing all invoices.
    /// </summary>
    public class InvoicesModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesModel" /> class.
        /// </summary>
        public InvoicesModel()
        {
            Invoices = new List<Invoice>(); 
        }

        /// <summary>
        /// Gets or sets the invoices.
        /// </summary>
        public List<Invoice> Invoices { get; }
    }
}