using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class ParticipantEngagementLevel
{
    public int EngagementLevelId { get; set; }

    public string EngagementCategory { get; set; }

    public string EngagementDescription { get; set; }

    /// <summary>
    /// The lower the number, the more desireable.
    /// </summary>
    public int EngagementRank { get; set; }

    public virtual ICollection<DailyMeetingParticipant> DailyMeetingParticipants { get; set; } = new List<DailyMeetingParticipant>();
}
