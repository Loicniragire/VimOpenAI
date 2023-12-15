using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeCommitMetric
{
    public string CodeCommitMetricsKey { get; set; }

    public int? LinesOfCode { get; set; }

    public decimal? CodeCoveragePercentage { get; set; }

    public int? CompletionTimeInMinutes { get; set; }

    public DateTime CommitDate { get; set; }

    public string CommitComment { get; set; }

    public virtual ICollection<CodeCommit> CodeCommits { get; set; } = new List<CodeCommit>();
}
