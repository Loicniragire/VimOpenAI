using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class ProjectStakeHolder
{
    public int ProjectStakeHolderId { get; set; }

    public string ProjectKey { get; set; }

    public string StakeHolderKey { get; set; }

    public virtual Project ProjectKeyNavigation { get; set; }

    public virtual StakeHolder StakeHolderKeyNavigation { get; set; }
}
