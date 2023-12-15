using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class ApplePayProvisioningData
    {
        [Required]
        public string CardToken { get; set; }

        [Required]
        [EnumDataType(typeof(DeviceType), ErrorMessage = "DeviceType is required and value must exist in the enum definition. Please reference Swagger page for defined values.")]
        public DeviceType DeviceType { get; set; }

        [Required]
        public string ProvisioningAppVersion { get; set; }

        [Required]
        public string LeafCertificate { get; set; }

        [Required]
        public string SubCACertificate { get; set; }

        [Required]
        public string Nonce { get; set; }

        [Required]
        public string NonceSignature { get; set; }
    }
}
