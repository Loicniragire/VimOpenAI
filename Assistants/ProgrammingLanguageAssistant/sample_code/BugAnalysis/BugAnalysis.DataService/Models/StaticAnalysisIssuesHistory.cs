using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StaticAnalysisIssuesHistory
{
    public int IssueHistoryId { get; set; }

    public string ProjectKey { get; set; }

    public DateTime AnalysisDate { get; set; }

    public int TotalIssuesFound { get; set; }

    public int? TotalCriticalIssues { get; set; }

    public int? TotalHighSeverityIssues { get; set; }

    public int? TotalMediumSeverityIssues { get; set; }

    public int? TotalLowSeverityIssues { get; set; }

    public virtual Project ProjectKeyNavigation { get; set; }
}
