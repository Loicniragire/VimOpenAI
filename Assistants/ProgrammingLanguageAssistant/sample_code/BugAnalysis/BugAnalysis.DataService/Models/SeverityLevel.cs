using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class SeverityLevel
{
    public int SeverityLevelId { get; set; }

    public string SeverityCategory { get; set; }

    public string SeverityDescription { get; set; }

    public virtual ICollection<Bug> Bugs { get; set; } = new List<Bug>();

    public virtual ICollection<StaticAnalysisIssue> StaticAnalysisIssues { get; set; } = new List<StaticAnalysisIssue>();
}
