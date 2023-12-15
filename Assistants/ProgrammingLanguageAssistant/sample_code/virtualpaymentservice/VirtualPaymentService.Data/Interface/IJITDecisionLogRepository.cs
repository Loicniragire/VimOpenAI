using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Data
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    public interface IJITDecisionLogRepository
    {
        /// <summary>
        /// Persists the data and decision for a JIT Funding request.
        /// </summary>
        /// <param name="decision">The <see cref="JITDecisionLog"/> data that was used/calculated during the JIT decision process.</param>
        Task SaveJITDecisionLogAsync(JITDecisionLog decision);
    }
}
