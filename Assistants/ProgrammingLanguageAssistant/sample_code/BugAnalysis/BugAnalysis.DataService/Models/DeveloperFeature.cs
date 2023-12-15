using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class DeveloperFeature
{
    public int DeveloperFeatureId { get; set; }

    public string DeveloperKey { get; set; }

    public string CodeFeatureKey { get; set; }

    public virtual ICollection<CodeCommit> CodeCommits { get; set; } = new List<CodeCommit>();

    public virtual CodeFeature CodeFeatureKeyNavigation { get; set; }

    public virtual ICollection<CodeReview> CodeReviews { get; set; } = new List<CodeReview>();

    public virtual Developer DeveloperKeyNavigation { get; set; }
}
