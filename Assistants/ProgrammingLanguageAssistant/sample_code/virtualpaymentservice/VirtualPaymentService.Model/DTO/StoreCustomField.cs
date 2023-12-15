using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class StoreCustomField
    {
        /// <summary>
        /// Store identifier related to the Lease.
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Parent store related to store of lease.
        /// </summary>
        /// <remarks>
        /// Ignored when generating response mimics LAPM behavior. 
        /// </remarks>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ParentStoreId { get; set; }

        /// <summary>
        /// Grandparent store related to store of lease.
        /// </summary>
        /// <remarks>
        /// Ignored when generating response mimics LAPM behavior. 
        /// </remarks>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int GrandparentStoreId { get; set; }

        /// <summary>
        /// Custom store field.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CustomFieldName { get; set; }

        /// <summary>
        /// Custom store field value.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CustomFieldValue { get; set; }

        /// <summary>
        /// Custom store field description.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }
    }
}
