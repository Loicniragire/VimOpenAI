using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class Project
{
    public string ProjectKey { get; set; }

    public DateTime KickoffDate { get; set; }

    public DateTime ProjectedDeliveryDate { get; set; }

    public string ProjectRepositoryUrl { get; set; }

    public virtual ICollection<Epic> Epics { get; set; } = new List<Epic>();

    public virtual ICollection<ProjectStakeHolder> ProjectStakeHolders { get; set; } = new List<ProjectStakeHolder>();

    public virtual ICollection<StaticAnalysisIssue> StaticAnalysisIssues { get; set; } = new List<StaticAnalysisIssue>();

    public virtual ICollection<StaticAnalysisIssuesHistory> StaticAnalysisIssuesHistories { get; set; } = new List<StaticAnalysisIssuesHistory>();
}
