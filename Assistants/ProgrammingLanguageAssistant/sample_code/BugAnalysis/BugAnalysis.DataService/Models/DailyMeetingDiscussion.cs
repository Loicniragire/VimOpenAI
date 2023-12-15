using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class DailyMeetingDiscussion
{
    public int MeetingDiscussionId { get; set; }

    public int DailyMeetingId { get; set; }

    public string DiscussionTopic { get; set; }

    public string DiscussionSummary { get; set; }

    public virtual DailyMeeting DailyMeeting { get; set; }

    public virtual ICollection<DailyMeetingDiscussionParticipant> DailyMeetingDiscussionParticipants { get; set; } = new List<DailyMeetingDiscussionParticipant>();
}
