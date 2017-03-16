// -----------------------------------------------------------------------
// <copyright file="IDataProtector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    /// <summary>
    /// Represents a data protection strategy.
    /// </summary>
    public interface IDataProtector
    {
        /// <summary>
        /// Protects the specified data by encrypting.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <returns>Base64 encoded string that represented the protected data.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="data"/> is empty or null.
        /// </exception>
        string Protect(string data);

        /// <summary>
        /// Unprotects the specified data, which was protected by the <see cref="Protect(string)"/> method.
        /// </summary>
        /// <param name="data">The cipher text data to unprotect.</param>
        /// <returns>The decrypted data in plaintext.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="data"/> is empty or null.
        /// </exception>
        string Unprotect(string data);
    }
}