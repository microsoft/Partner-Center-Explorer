// -----------------------------------------------------------------------
// <copyright file="NewUserModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for creating a new user.
    /// </summary>
    public class NewUserModel
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [Display(Name = "Display Name")]
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets the usage location.
        /// </summary>
        [Required]
        public string UsageLocation { get; set; }

        /// <summary>
        /// Gets or sets the name of the user principal.
        /// </summary>
        [Display(Name = "User Principal Name")]
        [Required]
        public string UserPrincipalName { get; set; }
    }
}