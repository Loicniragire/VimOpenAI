using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeBase
{
    public string CodeBaseKey { get; set; }

    public string Language { get; set; }

    public string LanguageVersion { get; set; }

    public string Framework { get; set; }

    public string FrameworkVersion { get; set; }

    public virtual ICollection<CodeBaseFile> CodeBaseFiles { get; set; } = new List<CodeBaseFile>();

    public virtual ICollection<CodeBaseLibrary> CodeBaseLibraries { get; set; } = new List<CodeBaseLibrary>();
}
