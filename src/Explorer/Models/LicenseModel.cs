// -----------------------------------------------------------------------
// <copyright file="LicenseModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    /// <summary>
    /// Model for licenses that can be assigned to users.
    /// </summary>
    public class LicenseModel
    {
        /// <summary>
        /// Gets or sets the consumed units.
        /// </summary>
        public int ConsumedUnits { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this license is assigned.
        /// </summary>
        public bool IsAssigned { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU part number.
        /// </summary>
        public string SkuPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Gets or sets the total units that can be allocated.
        /// </summary>
        public int TotalUnits { get; set; }
    }
}