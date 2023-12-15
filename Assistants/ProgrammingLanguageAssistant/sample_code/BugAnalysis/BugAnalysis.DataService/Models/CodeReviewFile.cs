using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeReviewFile
{
    public string CodeReviewFileKey { get; set; }

    public string CodeReviewKey { get; set; }

    public int CodeBaseFileId { get; set; }

    public virtual CodeBaseFile CodeBaseFile { get; set; }

    public virtual CodeReview CodeReviewKeyNavigation { get; set; }
}
