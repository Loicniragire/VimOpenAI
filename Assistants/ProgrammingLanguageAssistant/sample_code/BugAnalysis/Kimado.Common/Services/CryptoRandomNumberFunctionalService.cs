// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoRandomNumberFunctionalService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the CryptoRandomNumberFunctionalService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

using Kimado.Common.Contracts;
using Kimado.Core;
using Kimado.Core.Contracts;

namespace Kimado.Common.Services
{
    /// <summary>
    /// The crypto random number functional service.
    /// </summary>
    public class CryptoRandomNumberFunctionalService : FunctionalServiceBase, ICryptoRandomNumberFunctionalService
    {
        /// <summary>
        /// The alphabet.
        /// </summary>
        public readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// The specials.
        /// </summary>
        public readonly string Specials = "|~?-=+_@#$()[]{}.,";

        /// <summary>
        /// The numerics.
        /// </summary>
        public readonly string Numerics = "0123456789";

        /// <summary>
        /// The encoding.
        /// </summary>
        public readonly string Encoding = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandomNumberFunctionalService"/> class. 
        /// CTOR
        /// </summary>
        /// <param name="serviceContainer">
        /// The service Container.
        /// </param>
        public CryptoRandomNumberFunctionalService(IServiceContainer serviceContainer)
            : base(serviceContainer)
        {
        }

        /// <summary>
        /// Gets a random string containing the encodings characters
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// string containing the encodings characters
        /// </returns>
        public string GetBase62String(int length)
        {
            return this.GetString(length, this.Encoding);
        }

        /// <summary>
        /// Gets a string of random numbers
        /// </summary>
        /// <param name="length">
        /// Length of returned string
        /// </param>
        /// <returns>
        /// Random string
        /// </returns>
        public string GetBase62Numeric(int length)
        {
            return this.GetString(length, this.Numerics);
        }

        /// <summary>
        /// GetRandomPassword will generate a random password to meet the specified requirements
        /// </summary>
        /// <param name="length">Length of the password string to be returned</param>
        /// <param name="includeUppercase">If true, include at least one uppercase character</param>
        /// <param name="includeLowercase">If true, include at least one lowercase character</param>
        /// <param name="includeNumeric">If true, include at least one numeric digit</param>
        /// <param name="includeSpecial">If true, include at least special character</param>
        /// <returns>Returns a random string that includes all the required characters</returns>
        public string GetRandomPassword(
            int length,
            bool includeUppercase,
            bool includeLowercase,
            bool includeNumeric,
            bool includeSpecial)
        {
            int baseLen = length;
            int numericLen = 0;
            int specialLen = 0;
            int upperLen = 0;

            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] numberBuffer = new byte[4];
                if (includeNumeric)
                {
                    // Get a 32 bit number
                    rg.GetBytes(numberBuffer, 0, 4);
                    numericLen = (int)Math.Abs((BitConverter.ToInt32(numberBuffer, 0) % 4)) + 1;
                    baseLen -= numericLen;
                }

                if (includeSpecial)
                {
                    // Get a 32 bit number
                    rg.GetBytes(numberBuffer, 0, 4);
                    specialLen = (int)Math.Abs((BitConverter.ToUInt32(numberBuffer, 0) % 3)) + 1;
                    baseLen -= specialLen;
                }

                if (includeLowercase)
                {
                    // Get a 32 bit number
                    rg.GetBytes(numberBuffer, 0, 4);
                    upperLen = (int)Math.Abs((BitConverter.ToInt32(numberBuffer, 0) % (baseLen / 2))) + baseLen / 4;
                    baseLen -= upperLen;
                }

                // Generate password
                string basePassword = GetString(baseLen, this.Alphabet) + this.GetString(upperLen, this.Alphabet).ToUpper()
                                                                        + this.GetString(numericLen, this.Numerics)
                                                                        + this.GetString(specialLen, this.Specials);
                byte[] basePasswordBytes = System.Text.Encoding.ASCII.GetBytes(basePassword);

                // Jumble the password
                byte[] jumble = new byte[1024];
                rg.GetBytes(jumble, 0, jumble.Length);
                for (int index = 0; index < jumble.Length; index += 8)
                {
                    int srcIndex = ((int)Math.Abs(BitConverter.ToInt32(jumble, index))) % length;
                    int dstIndex = ((int)Math.Abs(BitConverter.ToInt32(jumble, index + 4))) % length;
                    byte b = basePasswordBytes[dstIndex];
                    basePasswordBytes[dstIndex] = basePasswordBytes[srcIndex];
                    basePasswordBytes[srcIndex] = b;
                }

                return System.Text.Encoding.ASCII.GetString(basePasswordBytes);
            }
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="set">
        /// The set.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetString(int length, string set)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[Math.Max(length, sizeof(Int32))];
                rg.GetBytes(rno);
                int randomvalue = BitConverter.ToInt32(rno, 0);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in rno)
                {
                    sb.Append(set[b % set.Length]);
                }

                return sb.ToString().Substring(0, length);
            }
        }
    }
}