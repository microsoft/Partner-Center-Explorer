// -----------------------------------------------------------------------
// <copyright file="CustomerModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using PartnerCenter.Models.Customers;

    /// <summary>
    /// Model that represents a customer from Partner Center.
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// Gets or sets the billing profile.
        /// </summary>
        public CustomerBillingProfile BillingProfile { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company profile.
        /// </summary>
        public CustomerCompanyProfile CompanyProfile { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the relationship to partner.
        /// </summary>
        public string RelationshipToPartner { get; set; }
    }
}