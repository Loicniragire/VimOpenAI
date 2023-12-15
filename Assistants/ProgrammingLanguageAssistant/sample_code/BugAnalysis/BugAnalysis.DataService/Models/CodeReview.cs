using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeReview
{
    public string CodeReviewKey { get; set; }

    public int DeveloperFeatureId { get; set; }

    public ulong Approved { get; set; }

    public DateTime ReviewStartDatetimeUtc { get; set; }

    public DateTime? ReviewCompleteDatetimeUtc { get; set; }

    public virtual ICollection<CodeReviewFile> CodeReviewFiles { get; set; } = new List<CodeReviewFile>();

    public virtual DeveloperFeature DeveloperFeature { get; set; }
}
