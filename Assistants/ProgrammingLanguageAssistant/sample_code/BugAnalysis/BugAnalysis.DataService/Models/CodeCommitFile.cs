using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeCommitFile
{
    public int CodeCommitFileId { get; set; }

    public string CodeCommitKey { get; set; }

    public int CodeBaseFileId { get; set; }

    public int CodeCommitFileActionId { get; set; }

    public virtual CodeBaseFile CodeBaseFile { get; set; }

    public virtual CodeCommitFileAction CodeCommitFileAction { get; set; }

    public virtual CodeCommit CodeCommitKeyNavigation { get; set; }
}
