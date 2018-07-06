// -----------------------------------------------------------------------
// <copyright file="EditUserModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for editing a user.
    /// </summary>
    public class EditUserModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditUserModel" /> class.
        /// </summary>
        public EditUserModel()
        {
            Licenses = new List<LicenseModel>(); 
        }

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
        /// Gets or sets licenses for the user.
        /// </summary>
        public List<LicenseModel> Licenses { get; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the usage location.
        /// </summary>
        public string UsageLocation { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user principal.
        /// </summary>
        [Display(Name = "User Principal Name")]
        [Required]
        public string UserPrincipalName { get; set; }
    }
}