using System;
using System.Collections.Generic;

namespace BugAnalysis.DataService.Models;

public partial class DailyMeeting
{
    public int DailyMeetingId { get; set; }

    public DateTime MeetingStartDatetimeUtc { get; set; }

    public DateTime MeetingEndDatetimeUtc { get; set; }

    public string MeetingSubject { get; set; }

    public virtual ICollection<DailyMeetingDiscussion> DailyMeetingDiscussions { get; set; } = new List<DailyMeetingDiscussion>();
}
