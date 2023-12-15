using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class DailyMeetingParticipant
{
    public int MeetingParticipantId { get; set; }

    public string DeveloperKey { get; set; }

    public string StakeHolderKey { get; set; }

    public int? EngagementLevelId { get; set; }

    public virtual ICollection<DailyMeetingDiscussionParticipant> DailyMeetingDiscussionParticipants { get; set; } = new List<DailyMeetingDiscussionParticipant>();

    public virtual Developer DeveloperKeyNavigation { get; set; }

    public virtual ParticipantEngagementLevel EngagementLevel { get; set; }

    public virtual StakeHolder StakeHolderKeyNavigation { get; set; }
}
