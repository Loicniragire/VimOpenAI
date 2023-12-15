using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StaticAnalysisIssue
{
    public int IssueId { get; set; }

    public string ProjectKey { get; set; }

    public int RuleId { get; set; }

    public int CodeBaseFileId { get; set; }

    public int LineNumber { get; set; }

    public int ColumnNumber { get; set; }

    public string IssueDescription { get; set; }

    public int SeverityLevelId { get; set; }

    public DateTime DateFound { get; set; }

    public DateTime? DateResolved { get; set; }

    public virtual CodeBaseFile CodeBaseFile { get; set; }

    public virtual Project ProjectKeyNavigation { get; set; }

    public virtual StaticAnalysisRule Rule { get; set; }

    public virtual SeverityLevel SeverityLevel { get; set; }
}
