using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StaticAnalysisRule
{
    public int RuleId { get; set; }

    /// <summary>
    /// Unique code for the rule, often provided by static analysis tools
    /// </summary>
    public string RuleCode { get; set; }

    public string RuleDescription { get; set; }

    public int RuleCategoryId { get; set; }

    public virtual StaticAnalysisRuleCategory RuleCategory { get; set; }

    public virtual ICollection<StaticAnalysisIssue> StaticAnalysisIssues { get; set; } = new List<StaticAnalysisIssue>();
}
