// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the CertificateService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;

using Kimado.Common.Contracts;
using Kimado.Core;
using Kimado.Core.Contracts;

namespace Kimado.Common.Services
{
    /// <summary>
    /// The certificate service.
    /// </summary>
    public class CertificateService : FunctionalServiceBase, ICertificateService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateService"/> class. 
        /// Default CTOR
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public CertificateService(IServiceContainer container)
            : base(container)
        {
        }

        /// <summary>
        /// GetCertificate retrieves a certificate from the certificate store by subjectName
        /// </summary>
        /// <param name="subjectName">String containing the subject name found in the certificate</param>
        /// <returns>Returns an X509 certificate</returns>
        public X509Certificate2 GetCertificateBySubjectName(string subjectName)
        {
            X509Certificate2 certificate = null;
            using (var store = new X509Store(StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates;
                DateTime now = DateTime.UtcNow;
                foreach (X509Certificate2 cert in certificates)
                {
                    // TODO: Research the certificate expiration methodology and update
                    if (cert.Subject.Contains($"CN={subjectName}") && now < cert.NotAfter && now > cert.NotBefore)
                    {
                        byte[] certBytes = cert.Export(X509ContentType.Pkcs12);
                        certificate = new X509Certificate2(certBytes);
                        break;
                    }
                }

                store.Close();
                if (certificate == null)
                {
                    throw new Exception(subjectName);
                }

                return certificate;
            }
        }
    }
}
