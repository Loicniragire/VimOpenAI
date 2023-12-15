using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class CodeRelease
{
    public string CodeReleaseKey { get; set; }

    public DateTime ReleaseDate { get; set; }

    public virtual ICollection<CodeReleaseFeature> CodeReleaseFeatures { get; set; } = new List<CodeReleaseFeature>();
}
