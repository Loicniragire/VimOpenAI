using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class DailyMeetingDiscussionParticipant
{
    public int DiscussionParticipantId { get; set; }

    public int MeetingParticipantId { get; set; }

    public int MeetingDiscussionId { get; set; }

    public string ParticipantDiscussionView { get; set; }

    public virtual DailyMeetingDiscussion MeetingDiscussion { get; set; }

    public virtual DailyMeetingParticipant MeetingParticipant { get; set; }
}
