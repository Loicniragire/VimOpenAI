using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StaticAnalysisRuleCategory
{
    public int RuleCategoryId { get; set; }

    public string RuleCategoryDescription { get; set; }

    public virtual ICollection<StaticAnalysisRule> StaticAnalysisRules { get; set; } = new List<StaticAnalysisRule>();
}
