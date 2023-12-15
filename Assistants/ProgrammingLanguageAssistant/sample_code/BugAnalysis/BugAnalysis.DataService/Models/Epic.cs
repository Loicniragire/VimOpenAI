using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class Epic
{
    public string EpicKey { get; set; }

    public string ProjectKey { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<CodeFeature> CodeFeatures { get; set; } = new List<CodeFeature>();

    public virtual Project ProjectKeyNavigation { get; set; }
}
