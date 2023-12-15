using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class StakeHolder
{
    public string StakeHolderKey { get; set; }

    public int StakeHolderDepartmentId { get; set; }

    public int StakeHolderPositionId { get; set; }

    public virtual ICollection<DailyMeetingParticipant> DailyMeetingParticipants { get; set; } = new List<DailyMeetingParticipant>();

    public virtual ICollection<ProjectStakeHolder> ProjectStakeHolders { get; set; } = new List<ProjectStakeHolder>();

    public virtual StakeHolderDepartment StakeHolderDepartment { get; set; }

    public virtual StakeHolderPosition StakeHolderPosition { get; set; }
}
