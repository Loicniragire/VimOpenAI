// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICryptoRandomNumberFunctionalService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the ICryptoRandomNumberFunctionalService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kimado.Core.Contracts;

namespace Kimado.Common.Contracts
{
    /// <summary>
    /// The CryptoRandomNumberFunctionalService interface.
    /// </summary>
    public interface ICryptoRandomNumberFunctionalService : IFunctionalService
    {
        /// <summary>
        /// GetBase62String will return a random string with the specified length
        /// </summary>
        /// <param name="length">Length of string to return</param>
        /// <returns>Returns a random base62 string</returns>
        string GetBase62String(int length);

        /// <summary>
        /// GetBase62Numeric returns a string of random numbers
        /// </summary>
        /// <param name="length">Length of string to return</param>
        /// <returns>Returns a random base62 string</returns>
        string GetBase62Numeric(int length);

        /// <summary>
        /// GetRandomPassword will generate a random password to meet the specified requirements
        /// </summary>
        /// <param name="length">Length of the password string to be returned</param>
        /// <param name="includeUppercase">If true, include at least one uppercase character</param>
        /// <param name="includeLowercase">If true, include at least one lowercase character</param>
        /// <param name="includeNumeric">If true, include at least one numeric digit</param>
        /// <param name="includeSpecial">If true, include at least special character</param>
        /// <returns>Returns a random string that includes all the required characters</returns>
        string GetRandomPassword(int length, bool includeUppercase, bool includeLowercase, bool includeNumeric, bool includeSpecial);
    }
}