// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Office
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a result from the Service Communication API.
    /// </summary>
    /// <typeparam name="T">Type return from the Service Communication API.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}" /> class.
        /// </summary>
        public Result()
        {
            Value = new List<T>();
        }

        /// <summary>
        /// Gets or sets the OData context.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }

        /// <summary>
        /// Gets or sets the value returned from the API.
        /// </summary>
        public List<T> Value { get; }
    }
}