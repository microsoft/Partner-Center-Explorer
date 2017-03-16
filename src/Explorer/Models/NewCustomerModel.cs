// -----------------------------------------------------------------------
// <copyright file="NewCustomerModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// Model for creating a new customer.
    /// </summary>
    public class NewCustomerModel
    {
        /// <summary>
        /// Gets or sets the first address line.
        /// </summary>
        [Display(Name = "Address Line 1")]
        [Required]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line.
        /// </summary>
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid email address specified.")]
        [Required]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        [Display(Name = "Customer Name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [Display(Name = "Phone Number")]
        [RegularExpression("^(1[\\-\\/\\.]?)?(\\((\\d{3})\\)|(\\d{3}))[\\-\\/\\.]?(\\d{3})[\\-\\/\\.]?(\\d{4})$", ErrorMessage = "Please specify a valid phone number.")]
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the domain prefix.
        /// </summary>
        [Display(Name = "Domain Prefix")]
        [Remote("IsDomainAvailable", "Domains", ErrorMessage = "Domain prefix already exists. Please try another prefix.")]
        [Required]
        public string PrimaryDomain { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [Required]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the supported states.
        /// </summary>
        public IEnumerable<string> SupportedStates { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        [Display(Name = "Zip Code")]
        [RegularExpression("^\\d{5}(-\\d{4})?$", ErrorMessage = "Please specify a valid zip code.")]
        [Required]
        public string ZipCode { get; set; }
    }
}