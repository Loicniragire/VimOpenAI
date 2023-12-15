// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICertificateService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the ICertificateService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

using Kimado.Core.Contracts;

namespace Kimado.Common.Contracts
{
    /// <summary>
    /// The CertificateService interface.
    /// </summary>
    public interface ICertificateService : IFunctionalService
    {
        /// <summary>
        /// GetCertificate retrieves a certificate from the certificate store by subjectName
        /// </summary>
        /// <param name="subjectName">String containing the subject name found in the certificate</param>
        /// <returns>Returns an X509 certificate</returns>
        X509Certificate2 GetCertificateBySubjectName(string subjectName);
    }
}