using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeFeature
{
    public string CodeFeatureKey { get; set; }

    public string EpicKey { get; set; }

    public DateTime? CodeStartDate { get; set; }

    public DateTime? CodeCompleteDate { get; set; }

    public virtual ICollection<CodeReleaseFeature> CodeReleaseFeatures { get; set; } = new List<CodeReleaseFeature>();

    public virtual DeveloperFeature DeveloperFeature { get; set; }

    public virtual Epic EpicKeyNavigation { get; set; }
}
