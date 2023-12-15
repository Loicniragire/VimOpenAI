using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses
{
    public abstract class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
