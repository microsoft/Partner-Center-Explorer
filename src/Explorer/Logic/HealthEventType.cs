// -----------------------------------------------------------------------
// <copyright file="HealthEventType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    /// <summary>
    /// Enumeration description possible health event types.
    /// </summary>
    public enum HealthEventType
    {
        /// <summary>
        /// The health event is from Azure Insights.
        /// </summary>
        Azure,

        /// <summary>
        /// The health event is from Office 365 Service Communications API.
        /// </summary>
        Office
    }
}