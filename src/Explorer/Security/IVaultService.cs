// -----------------------------------------------------------------------
// <copyright file="IVaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a secure mechanism for retrieving and store information. 
    /// </summary>
    public interface IVaultService
    {
        /// <summary>
        /// Gets a value indicating whether the vault service is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        string Get(string identifier);

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        Task<string> GetAsync(string identifier);

        /// <summary>
        /// Stores the specified value in the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be stored.</param>
        /// <param name="value">The value to stored.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// or 
        /// <paramref name="value"/> is empty or null.
        /// </exception>
        void Store(string identifier, string value);

        /// <summary>
        /// Stores the specified value in the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be stored.</param>
        /// <param name="value">The value to stored.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// or 
        /// <paramref name="value"/> is empty or null.
        /// </exception>
        Task StoreAsync(string identifier, string value);
    }
}