using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StakeHolderPosition
{
    public int StakeHolderPositionId { get; set; }

    public string Position { get; set; }

    public ulong HasManagementDuties { get; set; }

    public string PositionDescription { get; set; }

    public virtual ICollection<StakeHolder> StakeHolders { get; set; } = new List<StakeHolder>();
}
