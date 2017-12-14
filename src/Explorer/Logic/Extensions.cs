// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Provides useful methods used for validation.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets the text from the description attribute.
        /// </summary>
        /// <param name="value">The enumeration value associated with the attribute.</param>
        /// <returns>A <see cref="string"/> containing the text from the description attribute.</returns>
        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute attribute;

            try
            {
                attribute = value.GetType()
                    .GetField(value.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault() as DescriptionAttribute;

                return attribute == null ? value.ToString() : attribute.Description;
            }
            finally
            {
                attribute = null;
            }
        }

        /// <summary>
        /// Converts the string value to an instance of <see cref="SecureString"/>.
        /// </summary>
        /// <param name="value">The string value to be converted.</param>
        /// <returns>An instance of <see cref="SecureString"/> that represents the string.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is empty or null.
        /// </exception>
        public static SecureString ToSecureString(this string value)
        {
            SecureString secureValue = new SecureString();

            value.AssertNotEmpty(nameof(value));

            foreach (char c in value)
            {
                secureValue.AppendChar(c);
            }

            secureValue.MakeReadOnly();

            return secureValue;
        }

        /// <summary>
        /// Converts an instance of <see cref="SecureString"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="secureString">Secure string to be converted.</param>
        /// <returns>An instance of <see cref="string"/> that represents the <see cref="SecureString"/> value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="secureString"/> is null.
        /// </exception>
        public static string ToUnsecureString(this SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;

            secureString.AssertNotNull(nameof(secureString));

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Ensures that a string is not empty.
        /// </summary>
        /// <param name="nonEmptyString">The string to validate.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="nonEmptyString"/> is empty or null.
        /// </exception>
        public static void AssertNotEmpty(this string nonEmptyString, string caption)
        {
            if (string.IsNullOrWhiteSpace(nonEmptyString))
            {
                throw new ArgumentException($"{caption ?? "string"} is not set");
            }
        }

        /// <summary>
        /// Ensures that a given object is not null. Throws an exception otherwise.
        /// </summary>
        /// <param name="objectToValidate">The object we are validating.</param>
        /// <param name="caption">The name to report in the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="objectToValidate"/> is null.
        /// </exception>
        public static void AssertNotNull(this object objectToValidate, string caption)
        {
            if (objectToValidate == null)
            {
                throw new ArgumentNullException(caption);
            }
        }
    }
}