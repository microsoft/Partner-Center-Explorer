// -----------------------------------------------------------------------
// <copyright file="IHealthEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    /// <summary>
    /// Interface for health events obtained from Azure Insights and Office 365 Service Communications API.
    /// </summary>
    public interface IHealthEvent
    {
        /// <summary>
        /// Gets the type of the <see cref="IHealthEvent"/>.
        /// </summary>
        HealthEventType EventType { get; }

        /// <summary>
        /// Gets or sets the status of the <see cref="IHealthEvent"/>.
        /// </summary>
        string Status { get; set; }
    }
}