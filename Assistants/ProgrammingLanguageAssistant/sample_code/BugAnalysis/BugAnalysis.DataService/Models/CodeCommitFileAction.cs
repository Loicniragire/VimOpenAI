using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeCommitFileAction
{
    public int CodeCommitFileActionId { get; set; }

    public string FileAction { get; set; }

    public string FileActionDescription { get; set; }

    public virtual ICollection<CodeCommitFile> CodeCommitFiles { get; set; } = new List<CodeCommitFile>();
}
