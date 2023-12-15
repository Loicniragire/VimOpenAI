using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeCommit
{
    public string CodeCommitKey { get; set; }

    public string PreviousCommit { get; set; }

    public int DeveloperFeatureId { get; set; }

    public string CodeCommitMetricsKey { get; set; }

    public virtual ICollection<CodeCommitFile> CodeCommitFiles { get; set; } = new List<CodeCommitFile>();

    public virtual CodeCommitMetric CodeCommitMetricsKeyNavigation { get; set; }

    public virtual DeveloperFeature DeveloperFeature { get; set; }
}
