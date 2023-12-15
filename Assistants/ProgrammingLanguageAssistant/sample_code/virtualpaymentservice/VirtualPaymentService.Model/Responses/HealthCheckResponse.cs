using System;
using System.Collections.Generic;

namespace VirtualPaymentService.Model.Responses
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<SubHealthCheck> HealthChecks { get; set; }
        public string HealthCheckDuration { get; set; }
    }

    public class SubHealthCheck
    {
        public string Component { get; set; }
        public string Status { get; set; }
        public IReadOnlyDictionary<string, object> Results { get; set; }
    }
}
