using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeBaseFile
{
    public int CodeBaseFileId { get; set; }

    public string CodeBaseKey { get; set; }

    public string Filename { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual CodeBase CodeBaseKeyNavigation { get; set; }

    public virtual ICollection<CodeCommitFile> CodeCommitFiles { get; set; } = new List<CodeCommitFile>();

    public virtual ICollection<CodeReviewComment> CodeReviewComments { get; set; } = new List<CodeReviewComment>();

    public virtual ICollection<CodeReviewFile> CodeReviewFiles { get; set; } = new List<CodeReviewFile>();

    public virtual ICollection<StaticAnalysisIssue> StaticAnalysisIssues { get; set; } = new List<StaticAnalysisIssue>();
}
