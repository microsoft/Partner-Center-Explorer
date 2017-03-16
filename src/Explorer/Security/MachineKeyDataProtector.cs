// -----------------------------------------------------------------------
// <copyright file="MachineKeyDataProtector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.Text;
    using System.Web.Security;
    using Logic;

    /// <summary>
    /// Provides data protection using the machine encryption.
    /// </summary>
    internal sealed class MachineKeyDataProtector : IDataProtector
    {
        /// <summary>
        /// The purposes of the data being protected.
        /// </summary>
        private readonly string[] purposes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineKeyDataProtector"/> class.
        /// </summary>
        /// <param name="purposes">The purpose of the data being protected.</param>
        public MachineKeyDataProtector(string[] purposes)
        {
            this.purposes = purposes;
        }

        /// <summary>
        /// Protects the specified data by encrypting.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <returns>Base64 encoded string that represented the protected data.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="data"/> is empty or null.
        /// </exception>
        public string Protect(string data)
        {
            byte[] buffer;

            data.AssertNotEmpty(nameof(data));

            try
            {
                buffer = Encoding.ASCII.GetBytes(data);
                return Convert.ToBase64String(MachineKey.Protect(buffer, this.purposes));
            }
            finally
            {
                buffer = null;
            }
        }

        /// <summary>
        /// Unprotects the specified data, which was protected by the <see cref="Protect(string)"/> method.
        /// </summary>
        /// <param name="data">The cipher text data to unprotect.</param>
        /// <returns>The decrypted data in plaintext.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="data"/> is empty or null.
        /// </exception>
        public string Unprotect(string data)
        {
            byte[] buffer;
            byte[] decrypt;

            data.AssertNotEmpty(nameof(data));

            try
            {
                buffer = Convert.FromBase64String(data);
                decrypt = MachineKey.Unprotect(buffer, this.purposes);

                if (decrypt == null)
                {
                    throw new DataProtectorException("Unable to unprotect the specified data.");
                }

                return Encoding.ASCII.GetString(decrypt);
            }
            finally
            {
                buffer = null;
                decrypt = null;
            }
        }
    }
}